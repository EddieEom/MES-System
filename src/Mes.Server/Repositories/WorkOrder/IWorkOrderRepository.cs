using Mes.Server.Models.WorkOrder;

namespace Mes.Server.Repositories.WorkOrder;

public interface IWorkOrderRepository
{
    Task<WorkOrderCreateResult> CreateAsync(
        WorkOrderCreateRequest request
    );

    Task<Models.WorkOrder.WorkOrderInfo?> GetByCodeAsync(
        string workOrderCode
    );

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