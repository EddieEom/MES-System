namespace Mes.Gateway.Models.Quailty;

public class QualityResultApiRequest
{
    public Guid EventId { get; set; }

    public long ProductId { get; set; }

    public string Result { get; set; } = string.Empty;

    public string? SourceProductType { get; set; }

    public DateTimeOffset InspectedAt { get; set; }

    public string SourceSystem { get; set; } = "GEMINI";
}