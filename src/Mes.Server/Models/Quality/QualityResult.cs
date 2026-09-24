namespace Mes.Server.Models.Quality;

public class QualityResult
{
    public long QualityResultId { get; set; }

    public Guid EventId { get; set; }

    public long ProductId { get; set; }

    public int MachineId { get; set; }

    public string MachineCode { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string? SourceProductType { get; set; }

    public DateTimeOffset InspectedAt { get; set; }

    public string SourceSystem { get; set; } = "GEMINI";

    public DateTimeOffset ReceivedAt { get; set; }
}