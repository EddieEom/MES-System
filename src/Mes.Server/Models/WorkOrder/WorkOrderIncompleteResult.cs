namespace Mes.Server.Models.WorkOrder;

public class WorkOrderIncompleteResult
{
    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } = string.Empty;

    public int TargetQty { get; set; }

    public short LotCount { get; set; }

    public short LotSize { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }
}