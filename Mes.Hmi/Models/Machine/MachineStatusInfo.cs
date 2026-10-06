namespace Mes.Hmi.Models.Machine;

public class MachineStatusInfo
{
    public long MachineStatusHistoryId { get; set; }
    public int MachineId { get; set; }
    public string? MachineCode { get; set; }

    public DateTimeOffset MeasuredAt { get; set; }

    public string? State { get; set; }

    public int PartsEntered { get; set; }
    public int PartsExited { get; set; }
    public int PartsCurrent { get; set; }

    public double PartsAverageTime { get; set; }

    public double IdlePercentage { get; set; }
    public double BusyPercentage { get; set; }
    public double BlockedPercentage { get; set; }
    public double FailedPercentage { get; set; }
    public double RepairPercentage { get; set; }

    public double Utilization { get; set; }

    public string? SourceSystem { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }
}