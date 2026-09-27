using Mes.Gateway.Models.Control;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;

namespace Mes.Gateway.Services;

public sealed class GatewaySignalRService :
    IAsyncDisposable
{
    private readonly HubConnection _connection;

    private readonly PlcControlService
        _plcControlService;

    private readonly ConcurrentDictionary<
        Guid,
        byte
    > _processedCommands =
        new();


    public GatewaySignalRService(
        string serverBaseUrl,
        PlcControlService plcControlService)
    {
        _plcControlService =
            plcControlService;


        string hubUrl =
            $"{serverBaseUrl.TrimEnd('/')}/hubs/gateway";


        _connection =
            new HubConnectionBuilder()
                .WithUrl(
                    hubUrl
                )
                .WithAutomaticReconnect(
                    new[]
                    {
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    }
                )
                .Build();


        // ======================================
        // PLC Command 수신
        // ======================================

        _connection.On<PlcCommandMessage>(
            "PlcCommand",
            HandleCommandAsync
        );


        // ======================================
        // SignalR 상태 로그
        // ======================================

        _connection.Reconnecting +=
            error =>
            {
                Console.WriteLine(
                    $"[SIGNALR] 재연결 중 - {error?.Message}"
                );

                return Task.CompletedTask;
            };


        _connection.Reconnected +=
            connectionId =>
            {
                Console.WriteLine(
                    $"[SIGNALR] 재연결 성공 - {connectionId}"
                );

                return Task.CompletedTask;
            };


        _connection.Closed +=
            error =>
            {
                Console.WriteLine(
                    $"[SIGNALR] 연결 종료 - {error?.Message}"
                );

                return Task.CompletedTask;
            };
    }


    // ======================================
    // SignalR Start
    // ======================================

    public async Task StartAsync(
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(
            "[SIGNALR] Mes.Server 연결 시도..."
        );


        await _connection
            .StartAsync(
                cancellationToken
            );


        Console.WriteLine(
            $"[SIGNALR] Mes.Server 연결 성공 - {_connection.ConnectionId}"
        );
    }


    // ======================================
    // PLC Command 처리
    // ======================================

    private async Task HandleCommandAsync(
        PlcCommandMessage message)
    {
        // 동일 CommandId 중복 실행 방지
        if (!_processedCommands.TryAdd(
            message.CommandId,
            0))
        {
            Console.WriteLine(
                $"[SIGNALR] 중복 PLC Command 무시 - {message.CommandId}"
            );

            return;
        }


        Console.WriteLine();

        Console.WriteLine(
            $"[SIGNALR] PLC Command 수신 - {message.Command}"
        );

        Console.WriteLine(
            $"[SIGNALR] CommandId: {message.CommandId}"
        );


        try
        {
            bool result =
                message.Command
                    .Trim()
                    .ToUpperInvariant()
                switch
                {
                    "START" =>
                        await _plcControlService
                            .StartAsync(),

                    "STOP" =>
                        await _plcControlService
                            .StopAsync(),

                    "RESET" =>
                        await _plcControlService
                            .ResetAsync(),

                    _ =>
                        false
                };


            if (result)
            {
                Console.WriteLine(
                    $"[SIGNALR] PLC Command 처리 완료 - {message.Command}"
                );
            }
            else
            {
                Console.WriteLine(
                    $"[SIGNALR] PLC Command 처리 실패 - {message.Command}"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[SIGNALR] PLC Command 처리 오류 - {message.Command}: {ex.Message}"
            );
        }
    }


    // ======================================
    // SignalR Stop
    // ======================================

    public async Task StopAsync(
        CancellationToken cancellationToken = default)
    {
        if (_connection.State ==
            HubConnectionState.Disconnected)
        {
            return;
        }


        await _connection
            .StopAsync(
                cancellationToken
            );


        Console.WriteLine(
            "[SIGNALR] Mes.Server 연결 종료"
        );
    }


    public async ValueTask DisposeAsync()
    {
        await _connection
            .DisposeAsync();
    }
}