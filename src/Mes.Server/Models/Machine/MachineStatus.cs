namespace Mes.Server.Models.Machine;

public class MachineStatus
{
    public long MachineStatusHistoryId { get; set; }

    public int MachineId { get; set; }

    public string MachineCode { get; set; } = string.Empty;

    public DateTimeOffset MeasuredAt { get; set; }

    public string State { get; set; } = string.Empty;

    public int PartsEntered { get; set; }

    public int PartsExited { get; set; }

    public int PartsCurrent { get; set; }

    public decimal PartsAverageTime { get; set; }

    public decimal IdlePercentage { get; set; }

    public decimal BusyPercentage { get; set; }

    public decimal BlockedPercentage { get; set; }

    public decimal FailedPercentage { get; set; }

    public decimal RepairPercentage { get; set; }

    public decimal Utilization { get; set; }

    public string SourceSystem { get; set; } = "GEMINI";

    public DateTime ReceivedAt { get; set; }
}