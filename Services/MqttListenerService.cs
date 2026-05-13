using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using SmartLockSystem.Data;
using SmartLockSystem.Models;
using System.Text.Json;

namespace SmartLockSystem.Services;

/// <summary>
/// Dịch vụ chạy nền: Lắng nghe MQTT topic "face_verify" từ Python.
/// Khi Python nhận diện được mặt → gửi JSON lên MQTT → C# parse → Check DB → Mở cửa.
/// </summary>
public class MqttListenerService : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISmartLockService _smartLockService;
    private readonly LockStatusService _lockStatus;
    private IMqttClient _mqttClient;

    public MqttListenerService(
        IConfiguration config,
        IServiceProvider serviceProvider,
        ISmartLockService smartLockService,
        LockStatusService lockStatus)
    {
        _config = config;
        _serviceProvider = serviceProvider;
        _smartLockService = smartLockService;
        _lockStatus = lockStatus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        var host = _config["Mqtt:Host"];
        var port = int.Parse(_config["Mqtt:Port"] ?? "8883");
        var username = _config["Mqtt:Username"];
        var password = _config["Mqtt:Password"];

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(host, port)
            .WithCredentials(username, password)
            .WithTlsOptions(o => o.UseTls())
            .WithClientId($"SmartLock_Listener_{Guid.NewGuid()}")
            .WithCleanSession(true)
            .Build();

        // Khi nhận được message từ Python
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            Console.WriteLine($"[MQTT NHẬN] Topic: {topic} | Payload: {payload}");

            if (topic == "face_verify")
            {
                await HandleFaceVerify(payload);
            }
        };

        // Kết nối và subscribe
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_mqttClient.IsConnected)
                {
                    Console.WriteLine("[MQTT Listener] Đang kết nối tới HiveMQ...");
                    await _mqttClient.ConnectAsync(options, stoppingToken);
                    Console.WriteLine("[MQTT Listener] ✅ Connected!");

                    // Subscribe topic "face_verify"
                    await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                        .WithTopic("face_verify")
                        .Build(), stoppingToken);

                    Console.WriteLine("[MQTT Listener] 👂 Đang lắng nghe topic 'face_verify'...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MQTT Listener] ❌ Lỗi: {ex.Message}");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    /// <summary>
    /// Parse JSON từ Python và xử lý xác thực.
    /// Python gửi: {"face_code": "FACE_ID_JUN", "username": "Jun", "confidence": 0.56, "is_match": true, ...}
    /// </summary>
    private async Task HandleFaceVerify(string payload)
    {
        try
        {
            // Parse JSON từ Python
            var json = JsonDocument.Parse(payload);
            var root = json.RootElement;

            var faceCode = root.GetProperty("face_code").GetString() ?? "";
            var username = root.GetProperty("username").GetString() ?? "";
            var confidence = root.GetProperty("confidence").GetDouble();
            var isMatch = root.GetProperty("is_match").GetBoolean();

            Console.WriteLine($"[FACE DATA] Tên: {username} | Mã: {faceCode} | Độ tin cậy: {confidence:P1} | Match: {isMatch}");

            // Nếu Python báo không match → từ chối luôn
            if (!isMatch)
            {
                Console.WriteLine($"[FACE FAIL] ❌ Python báo không khớp: {username}");
                await LogAccess(faceCode, "FaceID", false, $"Khuôn mặt không khớp: {username} (confidence: {confidence:P1})");
                await SendFaceResult($"DENIED|{username}|NOT_MATCH");
                return;
            }

            // Tìm user trong Database bằng username
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SmartLockDbContext>();

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.AccessCode == username && u.IsActive);

            if (user != null)
            {
                Console.WriteLine($"[FACE OK] ✅ Xác nhận: {user.FullName} (confidence: {confidence:P1}) → MỞ CỬA!");

                // 1. Gửi MQTT mở cửa
                var unlockReq = new LockCommandRequest { DeviceId = "test_door", Unlock = true };
                await _smartLockService.SendLockCommandAsync(unlockReq);

                // 1.5 Cập nhật trạng thái real-time cho Frontend
                _lockStatus.SetUnlocked(user.FullName);

                // 2. Ghi lịch sử
                db.AccessLogs.Add(new AccessLog
                {
                    AccessCode = username,
                    AccessType = "FaceID",
                    IsSuccess = true,
                    Message = $"{user.FullName} đã mở cửa (confidence: {confidence:P1})",
                    UserId = user.Id,
                    DeviceId = 1,
                    Timestamp = DateTime.Now
                });
                await db.SaveChangesAsync();

                // 3. Phản hồi Python
                await SendFaceResult($"OK|{user.FullName}|UNLOCKED");
            }
            else
            {
                Console.WriteLine($"[FACE FAIL] ❌ Không có trong DB: {username}");

                // Ghi log truy cập trái phép
                await LogAccess(username, "FaceID", false, $"Người dùng không có trong hệ thống: {username}");
                await SendFaceResult($"DENIED|{username}|NOT_IN_DB");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[PARSE ERROR] Payload không phải JSON hợp lệ: {ex.Message}");
            // Fallback: thử xử lý như text thuần (tên khuôn mặt)
            await HandlePlainTextVerify(payload);
        }
    }

    /// <summary>
    /// Fallback: xử lý nếu Python gửi text thuần (VD: chỉ gửi "Jun")
    /// </summary>
    private async Task HandlePlainTextVerify(string faceName)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartLockDbContext>();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.AccessCode == faceName && u.IsActive);

        if (user != null)
        {
            Console.WriteLine($"[FACE OK] ✅ {user.FullName} → MỞ CỬA!");
            await _smartLockService.SendLockCommandAsync(
                new LockCommandRequest { DeviceId = "test_door", Unlock = true });

            db.AccessLogs.Add(new AccessLog
            {
                AccessCode = faceName, AccessType = "FaceID", IsSuccess = true,
                Message = $"{user.FullName} đã mở cửa thành công",
                UserId = user.Id, DeviceId = 1, Timestamp = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
            await SendFaceResult($"OK|{user.FullName}");
        }
        else
        {
            Console.WriteLine($"[FACE FAIL] ❌ Không tìm thấy: {faceName}");
            await LogAccess(faceName, "FaceID", false, $"Khuôn mặt không xác định: {faceName}");
            await SendFaceResult($"DENIED|{faceName}");
        }
    }

    private async Task LogAccess(string code, string type, bool success, string message)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartLockDbContext>();
        db.AccessLogs.Add(new AccessLog
        {
            AccessCode = code, AccessType = type, IsSuccess = success,
            Message = message, Timestamp = DateTime.Now
        });
        await db.SaveChangesAsync();
    }

    private async Task SendFaceResult(string result)
    {
        if (_mqttClient.IsConnected)
        {
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic("face_result")
                .WithPayload(result)
                .Build();
            await _mqttClient.PublishAsync(msg);
            Console.WriteLine($"[MQTT GỬI] face_result: {result}");
        }
    }
}
