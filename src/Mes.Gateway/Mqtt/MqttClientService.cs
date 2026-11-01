using System.Text;
using Mes.Gateway.Routing;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace Mes.Gateway.Mqtt;

/// <summary>One reconnect owner; stable MQTT 3.1.1 session and QoS 1 subscription.</summary>
public sealed class MqttClientService
{
    private readonly MqttFactory _factory = new();
    private readonly IMqttClient _client;
    private readonly Func<string, string, Task> _route;
    private readonly MqttClientOptions _options;
    private readonly CancellationTokenSource _stop = new();
    private Task? _connectionLoop;
    private long _received, _connections, _failures;
    private bool _started;
    private int _stopped;

    public long ReceivedCount => Interlocked.Read(ref _received);
    public long ConnectionCount => Interlocked.Read(ref _connections);
    public long HandlerFailureCount => Interlocked.Read(ref _failures);

    public MqttClientService(MqttMessageRouter router) : this(router.RouteAsync) { }

    // Injectable handler allows testing the actual receiver without PLC/COM/SQL Server.
    public MqttClientService(Func<string, string, Task> route,
        string? host = null, int? port = null, string? clientId = null)
    {
        _route = route;
        _client = _factory.CreateMqttClient();
        _options = new MqttClientOptionsBuilder()
            .WithClientId(clientId ?? Environment.GetEnvironmentVariable("MES_MQTT_CLIENT_ID") ?? "MES-GATEWAY-01")
            .WithTcpServer(host ?? Environment.GetEnvironmentVariable("MES_MQTT_HOST") ?? "127.0.0.1",
                port ?? int.Parse(Environment.GetEnvironmentVariable("MES_MQTT_PORT") ?? "1883"))
            .WithCleanSession(false)
            .WithTimeout(TimeSpan.FromSeconds(5))
            .Build();
        _client.ConnectedAsync += async _ =>
        {
            var subscription = _factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic("dt/#")
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)).Build();
            var result = await _client.SubscribeAsync(subscription, _stop.Token);
            if (result.Items.Any(i => (int)i.ResultCode >= 128))
                throw new InvalidOperationException("Broker rejected dt/# subscription.");
            Interlocked.Increment(ref _connections);
            Console.WriteLine("[MQTT] connected/subscribed QoS=1 cleanSession=false");
        };
        _client.ApplicationMessageReceivedAsync += async e =>
        {
            // MQTT ACK is a transport acknowledgement, not a DB commit receipt.
            e.AutoAcknowledge = false;
            try
            {
                await _route(e.ApplicationMessage.Topic,
                    Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                await e.AcknowledgeAsync(_stop.Token);
                Interlocked.Increment(ref _received);
            }
            catch (OperationCanceledException) when (_stop.IsCancellationRequested) { }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failures);
                Console.WriteLine($"[MQTT] handler failed; no ACK: {ex.Message}");
                // No automatic retry here: some handlers may already have performed side effects.
            }
        };
    }

    public Task StartAsync()
    {
        if (_started) throw new InvalidOperationException("MQTT service is single-use.");
        _started = true;
        _connectionLoop = RunConnectionLoopAsync();
        return Task.CompletedTask;
    }

    private async Task RunConnectionLoopAsync()
    {
        while (!_stop.IsCancellationRequested)
        {
            try
            {
                if (!_client.IsConnected)
                    await _client.ConnectAsync(_options, _stop.Token);
            }
            catch (OperationCanceledException) when (_stop.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                Console.WriteLine($"[MQTT] connect/subscribe failed; retry in 1s: {ex.Message}");
                // A rejected subscription must not leave a connected but unsubscribed client.
                if (_client.IsConnected)
                    await _client.DisconnectAsync();
            }
            try { await Task.Delay(1000, _stop.Token); }
            catch (OperationCanceledException) { break; }
        }
    }

    public async Task StopAsync()
    {
        if (Interlocked.Exchange(ref _stopped, 1) != 0) return;
        await _stop.CancelAsync();
        if (_connectionLoop != null) await _connectionLoop;
        if (_client.IsConnected) await _client.DisconnectAsync();
        _client.Dispose();
        _stop.Dispose();
    }
}
