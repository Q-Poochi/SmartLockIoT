using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using SmartLockSystem.Data;
using SmartLockSystem.Models;

namespace SmartLockSystem.Services;

/// <summary>
/// Dịch vụ chạy nền: Lắng nghe MQTT topic "face_verify" từ Python.
/// Khi Python nhận diện được mặt → gửi tên lên MQTT → C# nhận → Check DB → Mở cửa.
/// </summary>
public class MqttListenerService : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISmartLockService _smartLockService;
    private IMqttClient _mqttClient;

    public MqttListenerService(
        IConfiguration config,
        IServiceProvider serviceProvider,
        ISmartLockService smartLockService)
    {
        _config = config;
        _serviceProvider = serviceProvider;
        _smartLockService = smartLockService;
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

                    // Subscribe topic "face_verify" - Python sẽ gửi tên khuôn mặt vào đây
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

            await Task.Delay(5000, stoppingToken); // Retry mỗi 5 giây nếu mất kết nối
        }
    }

    /// <summary>
    /// Xử lý khi nhận được tên khuôn mặt từ Python.
    /// VD: Python gửi "Jun" → Check DB → Nếu có → Gửi "unlock" tới ESP32
    /// </summary>
    private async Task HandleFaceVerify(string faceName)
    {
        // Tạo scope mới để dùng DbContext (vì BackgroundService là Singleton)
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartLockDbContext>();

        // Tìm user trong Database bằng AccessCode = tên khuôn mặt
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.AccessCode == faceName && u.IsActive);

        if (user != null)
        {
            Console.WriteLine($"[FACE OK] ✅ Xác nhận: {user.FullName} → MỞ CỬA!");

            // 1. Gửi MQTT mở cửa
            var unlockReq = new LockCommandRequest { DeviceId = "test_door", Unlock = true };
            await _smartLockService.SendLockCommandAsync(unlockReq);

            // 2. Ghi lịch sử vào Database
            db.AccessLogs.Add(new AccessLog
            {
                AccessCode = faceName,
                AccessType = "FaceID",
                IsSuccess = true,
                Message = $"{user.FullName} đã mở cửa thành công",
                UserId = user.Id,
                DeviceId = 1,
                Timestamp = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            // 3. Gửi phản hồi về Python qua MQTT (optional)
            var responseMsg = new MqttApplicationMessageBuilder()
                .WithTopic("face_result")
                .WithPayload($"OK|{user.FullName}")
                .Build();
            await _mqttClient.PublishAsync(responseMsg);
        }
        else
        {
            Console.WriteLine($"[FACE FAIL] ❌ Không tìm thấy: {faceName}");

            // Ghi lại truy cập trái phép
            db.AccessLogs.Add(new AccessLog
            {
                AccessCode = faceName,
                AccessType = "FaceID",
                IsSuccess = false,
                Message = $"Khuôn mặt không xác định: {faceName}",
                Timestamp = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            // Gửi phản hồi từ chối
            var responseMsg = new MqttApplicationMessageBuilder()
                .WithTopic("face_result")
                .WithPayload($"DENIED|{faceName}")
                .Build();
            await _mqttClient.PublishAsync(responseMsg);
        }
    }
}
