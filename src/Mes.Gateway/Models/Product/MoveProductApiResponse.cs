namespace Mes.Gateway.Models.MoveProduct;

public class MoveProductApiResponse
{
    public string Status { get; set; } = string.Empty;

    public MoveProductResultData? Product { get; set; }
}


public class MoveProductResultData
{
    public long ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string QualityStatus { get; set; } = string.Empty;

    public string CurrentLocation { get; set; } = string.Empty;
}