using Mes.Gateway.Models.MachineStatus;

namespace Mes.Gateway.Services;

public class MesDashboardService
{
    private readonly MachineStatusService _machineStatusService;
    private readonly ProductionService _productionService;

    public MesDashboardService(
        MachineStatusService machineStatusService,
        ProductionService productionService)
    {
        _machineStatusService = machineStatusService;
        _productionService = productionService;
    }

    public void Print()
    {
        MachineStatusMessage? vm1 =
            _machineStatusService.Get("VM-1");

        MachineStatusMessage? press =
            _machineStatusService.Get("PRESS");


        Console.WriteLine();
        Console.WriteLine("==============================================");
        Console.WriteLine("               MES STATUS");
        Console.WriteLine("==============================================");


        // =====================================
        // VM-1
        // =====================================

        Console.WriteLine("[ VM-1 ]");

        if (vm1 != null)
        {
            Console.WriteLine($"State        : {vm1.State}");
            Console.WriteLine($"Entered      : {vm1.PartsEntered}");
            Console.WriteLine($"Exited       : {vm1.PartsExited}");
            Console.WriteLine($"Utilization  : {vm1.Utilization:F2}%");
        }
        else
        {
            Console.WriteLine("데이터 없음");
        }


        Console.WriteLine();


        // =====================================
        // PRESS
        // =====================================

        Console.WriteLine("[ PRESS ]");

        if (press != null)
        {
            Console.WriteLine($"State        : {press.State}");
            Console.WriteLine($"Entered      : {press.PartsEntered}");
            Console.WriteLine($"Exited       : {press.PartsExited}");
            Console.WriteLine($"Utilization  : {press.Utilization:F2}%");
        }
        else
        {
            Console.WriteLine("데이터 없음");
        }


        Console.WriteLine();


        // =====================================
        // PRODUCTION
        // =====================================

        Console.WriteLine("[ PRODUCTION ]");

        Console.WriteLine(
            $"Target       : {_productionService.TargetQty}"
        );

        Console.WriteLine(
            $"Produced     : {_productionService.ProducedQty}"
        );

        Console.WriteLine(
            $"Good         : {_productionService.GoodQty}"
        );

        Console.WriteLine(
            $"Defect       : {_productionService.DefectQty}"
        );

        Console.WriteLine(
            $"Achievement  : {_productionService.AchievementRate:F2}%"
        );

        Console.WriteLine(
            $"Yield        : {_productionService.YieldRate:F2}%"
        );


        Console.WriteLine("==============================================");
        Console.WriteLine();
    }
}