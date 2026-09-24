namespace Mes.Gateway.Models.MoveProduct;
// Gateway와 Server 프로젝트를 직접 참조시키지 않고 HTTP DTO만 따로 두는 구조임.
public class ProductApiResponse
{
    public long ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public long LotId { get; set; }

    public string LotCode { get; set; } = string.Empty;

    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } = string.Empty;

    public short SequenceNo { get; set; }

    public string Status { get; set; } = string.Empty;

    public string QualityStatus { get; set; } = string.Empty;

    public int? CurrentLocationId { get; set; }

    public string? CurrentLocationCode { get; set; }

    public string? CurrentLocationName { get; set; }
}