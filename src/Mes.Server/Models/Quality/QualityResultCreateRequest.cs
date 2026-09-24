namespace Mes.Server.Models.Quality;

// Post 요청 전용 DTO
public class QualityResultCreateRequest
{
    public Guid EventId { get; set; }

    public long ProductId { get; set; }

    public string Result { get; set; } = string.Empty;

    public string? SourceProductType { get; set; }

    public DateTimeOffset InspectedAt { get; set; }

    public string SourceSystem { get; set; } = "GEMINI";
}