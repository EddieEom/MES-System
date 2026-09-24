using System.Text.Json;
using Mes.Gateway.Models;
using Mes.Gateway.Services;

namespace Mes.Gateway.Routing;

public class MqttMessageRouter
{
    private readonly MachineStatusService _machineStatusService;
    private readonly QualityService _qualityService;
    private readonly ProductionService _productionService;
    private readonly MesDashboardService _dashboardService;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MqttMessageRouter(
    MachineStatusService machineStatusService, 
    QualityService qualityService, 
    ProductionService productionService,
    MesDashboardService dashboardService)
    {
        _machineStatusService = machineStatusService;

        _qualityService = qualityService;

        _productionService = productionService;

        _dashboardService = dashboardService;
    }

    public Task RouteAsync(
        string topic,
        string payload)
    {
        try
        {
            switch (topic)
            {
                case "dt/press/status":
                case "dt/vm1/status":

                    HandleMachineStatus(
                        topic,
                        payload);

                    break;


                case "dt/quality/result":

                    HandleQualityResult(
                        payload);

                    break;


                default:

                    Console.WriteLine(
                        $"[ROUTER] 처리되지 않은 Topic: {topic}"
                    );

                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[ROUTER ERROR] {ex.Message}"
            );
        }

        return Task.CompletedTask;
    }


    private void HandleMachineStatus(
        string topic,
        string payload)
    {
        var data =
            JsonSerializer.Deserialize<MachineStatusMessage>(
                payload,
                _jsonOptions
            );

        if (data == null)
        {
            Console.WriteLine(
                "[ROUTER] Machine Status 변환 실패"
            );

            return;
        }

        _machineStatusService.Update(data);

        _dashboardService.Print();

        Console.WriteLine();
        Console.WriteLine(
            "========== MACHINE STATUS =========="
        );

        Console.WriteLine(
            $"Topic        : {topic}"
        );

        Console.WriteLine(
            $"Timestamp    : {data.Timestamp}"
        );

        Console.WriteLine(
            $"Machine      : {data.Machine}"
        );

        Console.WriteLine(
            $"State        : {data.State}"
        );

        Console.WriteLine(
            $"Entered      : {data.PartsEntered}"
        );

        Console.WriteLine(
            $"Exited       : {data.PartsExited}"
        );

        Console.WriteLine(
            $"Current      : {data.PartsCurrent}"
        );

        Console.WriteLine(
            $"Average Time : {data.PartsAverageTime:F2}"
        );

        Console.WriteLine(
            $"Busy         : {data.BusyPercentage:F2}%"
        );

        Console.WriteLine(
            $"Idle         : {data.IdlePercentage:F2}%"
        );

        Console.WriteLine(
            $"Blocked      : {data.BlockedPercentage:F2}%"
        );

        Console.WriteLine(
            $"Utilization  : {data.Utilization:F2}%"
        );

        Console.WriteLine(
            "===================================="
        );
    }


    private void HandleQualityResult(
    string payload)
    {
        var data =
            JsonSerializer.Deserialize<QualityResultMessage>(
                payload,
                _jsonOptions
            );

        if (data == null)
        {
            Console.WriteLine(
                "[ROUTER] Quality Result 변환 실패"
            );

            return;
        }

        bool processed =
        _qualityService.Process(data);

        if (processed)
        {
            _productionService.PrintStatus();
            _dashboardService.Print();
        }

        Console.WriteLine();
        Console.WriteLine(
            "========== QUALITY RESULT =========="
        );

        Console.WriteLine(
            $"EventId   : {data.EventId}"
        );

        Console.WriteLine(
            $"Timestamp : {data.Timestamp}"
        );

        Console.WriteLine(
            $"Machine   : {data.Machine}"
        );

        Console.WriteLine(
            $"Result    : {data.Result}"
        );

        Console.WriteLine(
            $"Type      : {data.ProductType}"
        );

        Console.WriteLine(
            "===================================="
        );
    }
}