using Mes.Server.Models.WorkOrder;

namespace Mes.Server.Services.WorkOrder;

public interface IWorkOrderService
{
    Task<WorkOrderStartResult> StartAsync(
        string workOrderCode
    );

    Task<WorkOrderStateResult> PauseAsync(
        string workOrderCode
    );

    Task<WorkOrderStateResult> ResumeAsync(
        string workOrderCode
    );
}