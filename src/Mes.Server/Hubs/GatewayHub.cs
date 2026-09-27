using Microsoft.AspNetCore.SignalR;

namespace Mes.Server.Hubs;

public class GatewayHub : Hub
{
    private const string GatewayGroup =
        "MES_GATEWAYS";


    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GatewayGroup
        );


        Console.WriteLine(
            $"[SIGNALR] Gateway 연결 - {Context.ConnectionId}"
        );


        await base.OnConnectedAsync();
    }


    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        Console.WriteLine(
            $"[SIGNALR] Gateway 연결 종료 - {Context.ConnectionId}"
        );


        await base.OnDisconnectedAsync(
            exception
        );
    }


    public static string GetGatewayGroup()
    {
        return GatewayGroup;
    }
}