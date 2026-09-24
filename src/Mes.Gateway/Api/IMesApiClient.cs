using Mes.Gateway.Models.MachineStatus;
using Mes.Gateway.Models.MoveProduct;
using Mes.Gateway.Models.Quailty;

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
}