using Mes.Server.Models.Quality;

namespace Mes.Server.Repositories.Quality;

public interface IQualityResultRepository
{
    Task<QualityProcessResult> RecordAsync(
        QualityResultCreateRequest request
    );

    Task<QualityResult?> GetByEventIdAsync(
        Guid eventId
    );
}