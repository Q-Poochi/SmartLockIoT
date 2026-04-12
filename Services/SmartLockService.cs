using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter; // Không có dòng này là không lấy được V5 đâu
using SmartLockSystem.Models;
using SmartLockSystem.Services;
using System.Text.Json;

namespace SmartLockSystem.Services;

public class SmartLockService : ISmartLockService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;

    public SmartLockService()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        // Cập nhật Meta: Kết nối HiveMQ bằng chuẩn MQTT v5
        _mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("broker.hivemq.com", 1883)
            .WithProtocolVersion(MqttProtocolVersion.V500) // <==== CHÍNH LÀ NÓ! Đẳng cấp V5.
            .WithClientId($"SmartLockApi_{Guid.NewGuid()}")
            .WithCleanSession(true) // Đám mây không lưu rác sau khi sập
            .Build();

        // Meta .NET 8: Tự động sơ cứu khi đứt cáp mạng mập cắn
        _mqttClient.DisconnectedAsync += async e =>
        {
            Console.WriteLine("[MQTT v5.0] Oops! Mất mạng trạm trung chuyển! Đang thử nối lại...");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
        };
    }

    public async Task<bool> SendLockCommandAsync(LockCommandRequest request)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                Console.WriteLine("[MQTT v5.0] Đang khởi động đường truyền tới HiveMQ...");
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                Console.WriteLine("[MQTT v5.0] Trạm sẵn sàng!");
            }

            var payload = new
            {
                command = request.Unlock ? "UNLOCK" : "LOCK",
                timestamp = DateTime.UtcNow,
                sender = "Backend API"
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            string topic = $"my_smartlock_v1/{request.DeviceId}/command";

            // Đóng gói bản tin chuẩn V5
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(jsonPayload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                // Các tính năng siêu cấp của v5 như User Properties có thể ráp thêm vào đây
                .Build();

            await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            Console.WriteLine($"[MQTT v5.0] Bắn tín hiệu -> {topic}: {jsonPayload}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MQTT Báo Động] Lỗi cmnr: {ex.Message}");
            return false;
        }
    }
}
