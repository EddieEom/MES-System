using Mes.Server.Hubs;
using Mes.Server.Models.Control;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Mes.Server.Controllers.Control;

[ApiController]
[Route("api/plc-control")]
public class PlcControlController : ControllerBase
{
    private readonly IHubContext<GatewayHub>
        _hubContext;


    public PlcControlController(
        IHubContext<GatewayHub> hubContext)
    {
        _hubContext =
            hubContext;
    }


    // ======================================
    // START
    // ======================================

    [HttpPost("start")]
    public async Task<IActionResult> Start()
    {
        return await SendCommandAsync(
            "START"
        );
    }


    // ======================================
    // STOP
    // ======================================

    [HttpPost("stop")]
    public async Task<IActionResult> Stop()
    {
        return await SendCommandAsync(
            "STOP"
        );
    }


    // ======================================
    // RESET
    // ======================================

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        return await SendCommandAsync(
            "RESET"
        );
    }


    // ======================================
    // SignalR 전송
    // ======================================

    private async Task<IActionResult>
        SendCommandAsync(
            string command)
    {
        var message =
            new PlcCommandMessage
            {
                CommandId =
                    Guid.NewGuid(),

                Command =
                    command,

                RequestedAt =
                    DateTimeOffset.UtcNow
            };


        await _hubContext
            .Clients
            .Group(
                GatewayHub
                    .GetGatewayGroup()
            )
            .SendAsync(
                "PlcCommand",
                message
            );


        Console.WriteLine(
            $"[PLC CONTROL API] {command} 전송 - {message.CommandId}"
        );


        return Ok(
            new
            {
                status = "sent",
                commandId =
                    message.CommandId,

                command =
                    message.Command,

                requestedAt =
                    message.RequestedAt
            }
        );
    }
}