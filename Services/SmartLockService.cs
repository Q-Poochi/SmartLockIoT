using MQTTnet;
using MQTTnet.Client;
using SmartLockSystem.Models;

namespace SmartLockSystem.Services;

public class SmartLockService : ISmartLockService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly IConfiguration _config;

    public SmartLockService(IConfiguration configuration)
    {
        _config = configuration;
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        // Đọc từ appsettings.json → mục "Mqtt"
        var host = _config["Mqtt:Host"];
        var port = int.Parse(_config["Mqtt:Port"] ?? "8883");
        var username = _config["Mqtt:Username"];
        var password = _config["Mqtt:Password"];

        _mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(host, port)
            .WithCredentials(username, password)
            .WithTlsOptions(o => o.UseTls())
            .WithClientId($"SmartLockApi_{Guid.NewGuid()}")
            .WithCleanSession(true)
            .Build();

        Console.WriteLine($"[MQTT Config] Host={host}, Port={port}, User={username}");
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

            // Đọc topic và payload từ appsettings.json
            string targetTopic = _config["Mqtt:Topic"] ?? "test_door";
            string strPayload = request.Unlock
                ? (_config["Mqtt:UnlockPayload"] ?? "unlock")
                : (_config["Mqtt:LockPayload"] ?? "lock");

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
