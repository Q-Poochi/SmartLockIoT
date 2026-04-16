using MQTTnet;
using MQTTnet.Client;
using SmartLockSystem.Models;

using SmartLockSystem.Services;

namespace SmartLockSystem.Services;

public class SmartLockService : ISmartLockService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;

    public SmartLockService()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        // 1. CẬP NHẬT TRẠM HIVEMQ CLOUD (Giáp SSL và Thông tin đăng nhập)
        _mqttOptions = new MqttClientOptionsBuilder()
            // Lưu ý: Cổng chuẩn của MQTT TLS thường là 8883 (Dân Python bên kia code 8884 coi chừng nhầm sang WebSockets nhé! Nên cứ để mặc định 8883 cho C# TCP là an toàn nhất)
            .WithTcpServer("778192bafdb24249954652d9ae05565d.s1.eu.hivemq.cloud", 8883)
            .WithCredentials("esp32", "Aabc12345") 
            .WithTlsOptions(o => o.UseTls())      
            .WithClientId($"SmartLockApi_{Guid.NewGuid()}")
            .WithCleanSession(true)
            .Build();
    }

    public async Task<bool> SendLockCommandAsync(LockCommandRequest request)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                Console.WriteLine("[MQTT] Đang kết nối tới HiveMQ Cloud...");
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                Console.WriteLine("[MQTT] Connected Thành Công!");
            }

            // 2. KHÔNG DÙNG JSON NỮA! Chỉ gửi chữ "ON" hoặc "OFF" theo đúng ý Python bên kia.
            string strPayload = request.Unlock ? "ON" : "OFF";

            // 3. FIX CỨNG TOPIC LÀ "esp32/led/control"
            string targetTopic = "esp32/led";

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(targetTopic)
                .WithPayload(strPayload) 
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            Console.WriteLine($"[Đã bắn thành công] -> [{targetTopic}] nội dung: {strPayload}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI MQTT] Văng mất mạng rồi: {ex.Message}");
            return false;
        }
    }
}
