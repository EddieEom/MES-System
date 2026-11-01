using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Mes.Gateway.Mqtt;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using MQTTnet.Protocol;

var factory = new MqttFactory();
using var broker = factory.CreateMqttServer(new MqttServerOptionsBuilder()
    .WithPersistentSessions().WithDefaultEndpoint().WithDefaultEndpointPort(21883).Build());
await using var proxy = new FaultProxy(21884, 21883);
var seen = new ConcurrentDictionary<int, byte>();
long duplicates = 0;
var receiver = new MqttClientService((_, payload) =>
{
    if (!seen.TryAdd(int.Parse(payload), 0)) Interlocked.Increment(ref duplicates);
    return Task.CompletedTask;
}, "127.0.0.1", 21884, "reliability-test");
await receiver.StartAsync();
await Task.Delay(1500); // initial broker unavailable: reconnect loop must recover
await broker.StartAsync();
proxy.Resume();
using var publisher = factory.CreateMqttClient();
await publisher.ConnectAsync(new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1", 21883).Build());
try
{
    await WaitFor(() => receiver.ConnectionCount >= 1, "initial subscription");
    int count = int.Parse(Environment.GetEnvironmentVariable("MES_TEST_COUNT") ?? "10000");
    int rate = int.Parse(Environment.GetEnvironmentVariable("MES_TEST_RATE") ?? "1000");
    if (count < 1 || rate < 1) throw new ArgumentException("count/rate must be positive");
    var timer = Stopwatch.StartNew();
    for (int id = 0; id < count; id++)
    {
        await Publish(id);
        var delay = TimeSpan.FromSeconds((id + 1.0) / rate) - timer.Elapsed;
        if (delay > TimeSpan.Zero) await Task.Delay(delay);
    }
    await WaitFor(() => receiver.ReceivedCount >= count, "load delivery", 120);
    var loadSeconds = timer.Elapsed.TotalSeconds;

    // Only receiver network is cut; publisher and broker remain online.
    await proxy.PauseAsync();
    await Task.Delay(1000); // let broker observe TCP EOF
    for (int id = count; id < count + 100; id++) await Publish(id);
    await Task.Delay(1000);
    if (seen.Keys.Any(id => id >= count)) throw new Exception("fault injection did not isolate receiver");
    proxy.Resume();
    await WaitFor(() => receiver.ReceivedCount >= count + 100, "offline session replay", 30);
    if (Enumerable.Range(0, count + 100).Any(id => !seen.ContainsKey(id)))
        throw new Exception("missing event IDs");
    await WaitFor(() => receiver.ConnectionCount >= 2, "resubscription after replay", 30);
    await receiver.StopAsync();
    var connectionsAtStop = receiver.ConnectionCount;
    await Task.Delay(1500);
    if (receiver.ConnectionCount != connectionsAtStop) throw new Exception("reconnected after stop");
    var report = new
    {
        utc = DateTimeOffset.UtcNow, runtime = Environment.Version.ToString(),
        test = "real Gateway MQTT receiver + in-process broker + TCP fault proxy + ID sink",
        targetMessagesPerSecond = rate, loadMessages = count,
        loadSeconds, observedMessagesPerSecond = count / loadSeconds,
        offlineMessages = 100, uniqueReceived = seen.Count, duplicates,
        connections = receiver.ConnectionCount, handlerFailures = receiver.HandlerFailureCount,
        missing = 0,
        scope = "No PLC, production router, HTTP API, DB or WinForms; not an end-to-end benchmark"
    };
    var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
    Directory.CreateDirectory("TestResults");
    await File.WriteAllTextAsync("TestResults/mqtt-reliability.json", json);
    Console.WriteLine(json);
}
finally { await publisher.DisconnectAsync(); await broker.StopAsync(); }

async Task Publish(int id)
{
    await publisher.PublishAsync(new MqttApplicationMessageBuilder().WithTopic("dt/test/event")
        .WithPayload(id.ToString()).WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build());
}
static async Task WaitFor(Func<bool> condition, string label, int seconds = 15)
{
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));
    while (!condition())
    {
        try { await Task.Delay(20, timeout.Token); }
        catch (OperationCanceledException) { throw new Exception($"Timeout: {label}"); }
    }
}

// A real TCP interruption, without deleting the broker's persistent session.
sealed class FaultProxy(int listenPort, int targetPort) : IAsyncDisposable
{
    private readonly List<TcpClient> _clients = [];
    private readonly List<Task> _pumps = [];
    private CancellationTokenSource? _cts;
    private TcpListener? _listener;
    private Task? _accept;
    public void Resume()
    {
        _cts = new();
        _listener = new TcpListener(IPAddress.Loopback, listenPort);
        _listener.Start();
        _accept = AcceptAsync(_cts.Token);
    }
    private async Task AcceptAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var incoming = await _listener!.AcceptTcpClientAsync(token);
                var upstream = new TcpClient();
                lock (_clients) { _clients.Add(incoming); _clients.Add(upstream); }
                await upstream.ConnectAsync(IPAddress.Loopback, targetPort, token);
                _pumps.Add(PumpAsync(incoming, upstream, token));
            }
        }
        catch (OperationCanceledException) { }
        catch (SocketException) when (token.IsCancellationRequested) { }
    }
    private static async Task PumpAsync(TcpClient a, TcpClient b, CancellationToken token)
    {
        try
        {
            var ab = a.GetStream().CopyToAsync(b.GetStream(), token);
            var ba = b.GetStream().CopyToAsync(a.GetStream(), token);
            await Task.WhenAny(ab, ba);
            a.Dispose(); b.Dispose();
            await Task.WhenAll(ab, ba);
        }
        catch (Exception e) when (e is IOException or SocketException or OperationCanceledException or ObjectDisposedException) { }
    }
    public async Task PauseAsync()
    {
        if (_cts == null) return;
        await _cts.CancelAsync(); _listener!.Stop();
        if (_accept != null) await _accept;
        lock (_clients) { foreach (var c in _clients) c.Dispose(); _clients.Clear(); }
        await Task.WhenAll(_pumps); _pumps.Clear();
        _cts.Dispose(); _cts = null;
    }
    public async ValueTask DisposeAsync() => await PauseAsync();
}
