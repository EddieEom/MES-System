namespace Mes.Gateway.Routing;
// The suite injects its sink instead; production router/API/DB are outside this test.
public class MqttMessageRouter
{
    public Task RouteAsync(string topic, string payload) => throw new NotSupportedException();
}
