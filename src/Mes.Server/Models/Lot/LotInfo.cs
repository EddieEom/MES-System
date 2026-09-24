namespace Mes.Server.Models.Lot;

public class LotInfo
{
    public long LotId { get; set; }

    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } = string.Empty;

    public string LotCode { get; set; } = string.Empty;

    public short LotSequence { get; set; }

    public short TargetQty { get; set; }

    public string Status { get; set; } = string.Empty;

    public int ProcessedQty { get; set; }

    public int GoodQty { get; set; }

    public int DefectQty { get; set; }

    public int RemainingQty =>
        Math.Max(0, TargetQty - ProcessedQty);

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }
}