namespace Mes.Server.Models.WorkOrder;

public class WorkOrderInfo
{
    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } = string.Empty;

    public int TargetQty { get; set; }

    public short LotCount { get; set; }

    public short LotSize { get; set; }

    public string Status { get; set; } = string.Empty;

    public long? CreatedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }
}