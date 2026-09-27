using Mes.Gateway.Models.MachineStatus;
using Mes.Gateway.Models.MoveProduct;
using Mes.Gateway.Models.Quailty;
using Mes.Gateway.Models.Production;
using Mes.Gateway.Models.Alarm;

namespace Mes.Gateway.Api;

public interface IMesApiClient
{
    // ======================================
    // Server Health
    // ======================================

    Task<bool> CheckConnectionAsync(
        CancellationToken cancellationToken = default
    );


    // ======================================
    // Machine Status
    // ======================================

    Task<bool> PostMachineStatusAsync(
        MachineStatusMessage message,
        CancellationToken cancellationToken = default
    );


    // ======================================
    // Product
    // ======================================

    Task<ProductApiResponse?> GetCurrentQualityProductAsync(
        CancellationToken cancellationToken = default
    );


    Task<ProductApiResponse?> GetCurrentProcessProductAsync(
        CancellationToken cancellationToken = default
    );


    // ======================================
    // Quality
    // ======================================

    Task<bool> PostQualityResultAsync(
        QualityResultMessage message,
        long productId,
        CancellationToken cancellationToken = default
    );


    // ======================================
    // Process
    // ======================================

    Task<bool> PostProcessEventAsync(
        long productId,
        string productCode,
        string toLocationCode,
        string eventType = "MOVE",
        DateTimeOffset? eventTime = null,
        string? remarks = null,
        Guid? eventId = null,
        CancellationToken cancellationToken = default
    );

    // ======================================
    // Production
    // ======================================

    Task<CurrentProductionApiResponse?>
    GetCurrentProductionAsync(
        CancellationToken cancellationToken = default
    );


    // ======================================
    // Alarm
    // ======================================

    Task<AlarmApiResponse?> PostAlarmAsync(
        AlarmCreateApiRequest request,
        CancellationToken cancellationToken = default
    );


    Task<IReadOnlyList<AlarmApiResponse>?>
        GetActiveAlarmsAsync(
            CancellationToken cancellationToken = default
        );


    Task<IReadOnlyList<AlarmApiResponse>?>
        GetMachineAlarmsAsync(
            string machineCode,
            CancellationToken cancellationToken = default
        );


    Task<bool> ClearAlarmAsync(
        long alarmId,
        CancellationToken cancellationToken = default
    );
}