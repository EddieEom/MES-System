namespace Mes.Server.Models.Process;

public class MoveProductResult
{
    public long ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string QualityStatus { get; set; } = string.Empty;

    public string CurrentLocation { get; set; } = string.Empty;
}