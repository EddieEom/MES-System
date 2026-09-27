using Mes.Gateway.Plc;

namespace Mes.Gateway.Services;

public sealed class PlcControlService
{
    private readonly IPlcClient _plcClient;

    // 현재 시뮬레이션용 Device Map
    // 실제 PLC 주소표 변경 시 여기에서 변경함.
    private const string StartDevice = "M100";
    private const string StopDevice = "M101";
    private const string ResetDevice = "M102";

    private const int CommandPulseMilliseconds = 300;


    public PlcControlService(
        IPlcClient plcClient)
    {
        _plcClient = plcClient;
    }


    // ======================================
    // START
    // ======================================

    public async Task<bool> StartAsync(
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(
            "[PLC CONTROL] START 명령"
        );


        bool result =
            await _plcClient
                .PulseDeviceAsync(
                    StartDevice,
                    CommandPulseMilliseconds,
                    cancellationToken
                );


        Console.WriteLine(
            result
                ? "[PLC CONTROL] START 명령 완료"
                : "[PLC CONTROL] START 명령 실패"
        );


        return result;
    }


    // ======================================
    // STOP
    // ======================================

    public async Task<bool> StopAsync(
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(
            "[PLC CONTROL] STOP 명령"
        );


        bool result =
            await _plcClient
                .PulseDeviceAsync(
                    StopDevice,
                    CommandPulseMilliseconds,
                    cancellationToken
                );


        Console.WriteLine(
            result
                ? "[PLC CONTROL] STOP 명령 완료"
                : "[PLC CONTROL] STOP 명령 실패"
        );


        return result;
    }


    // ======================================
    // RESET
    // ======================================

    public async Task<bool> ResetAsync(
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(
            "[PLC CONTROL] RESET 명령"
        );


        bool result =
            await _plcClient
                .PulseDeviceAsync(
                    ResetDevice,
                    CommandPulseMilliseconds,
                    cancellationToken
                );


        Console.WriteLine(
            result
                ? "[PLC CONTROL] RESET 명령 완료"
                : "[PLC CONTROL] RESET 명령 실패"
        );


        return result;
    }
}