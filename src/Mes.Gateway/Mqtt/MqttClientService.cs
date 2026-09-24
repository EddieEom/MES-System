using System.Text;
using Mes.Gateway.Routing;
using MQTTnet;
using MQTTnet.Client;

namespace Mes.Gateway.Mqtt;

public class MqttClientService
{
    private readonly MqttFactory _mqttFactory;
    private readonly IMqttClient _mqttClient;
    private readonly MqttMessageRouter _router;


    public MqttClientService(
        MqttMessageRouter router)
    {
        _router = router;

        _mqttFactory = new MqttFactory();

        _mqttClient =
            _mqttFactory.CreateMqttClient();


        _mqttClient.ApplicationMessageReceivedAsync
            += OnMessageReceivedAsync;


        _mqttClient.ConnectedAsync
            += OnConnectedAsync;


        _mqttClient.DisconnectedAsync
            += OnDisconnectedAsync;
    }


    public async Task StartAsync()
    {
        var options =
            new MqttClientOptionsBuilder()
                .WithClientId(
                    "MES-GATEWAY-01"
                )
                .WithTcpServer(
                    "127.0.0.1",
                    1883
                )
                .WithCleanSession()
                .Build();


        Console.WriteLine(
            "[MQTT] Broker 연결 시도..."
        );


        await _mqttClient.ConnectAsync(
            options,
            CancellationToken.None
        );
    }


    public async Task StopAsync()
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
        }
    }


    private async Task OnConnectedAsync(
        MqttClientConnectedEventArgs e)
    {
        Console.WriteLine(
            "[MQTT] Broker 연결 성공"
        );


        var subscribeOptions =
            _mqttFactory
                .CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("dt/#");
                    })
                .Build();


        await _mqttClient.SubscribeAsync(
            subscribeOptions,
            CancellationToken.None
        );


        Console.WriteLine(
            "[MQTT] dt/# 구독 시작"
        );
    }


    private async Task OnMessageReceivedAsync(
        MqttApplicationMessageReceivedEventArgs e)
    {
        string topic =
            e.ApplicationMessage.Topic;


        string payload =
            Encoding.UTF8.GetString(
                e.ApplicationMessage.PayloadSegment
            );


        Console.WriteLine();
        Console.WriteLine(
            $"[MQTT] 수신 → {topic}"
        );


        await _router.RouteAsync(
            topic,
            payload
        );
    }


    private Task OnDisconnectedAsync(
        MqttClientDisconnectedEventArgs e)
    {
        Console.WriteLine(
            "[MQTT] Broker 연결 해제"
        );

        return Task.CompletedTask;
    }
}