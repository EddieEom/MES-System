namespace Mes.Gateway.Models.MachineStatus;

public class MachineStatusApiRequest
{
    public string MachineCode { get; set; } = string.Empty;

    public DateTimeOffset MeasuredAt { get; set; }

    public string State { get; set; } = string.Empty;

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

    public string SourceSystem { get; set; } = "GEMINI";
}