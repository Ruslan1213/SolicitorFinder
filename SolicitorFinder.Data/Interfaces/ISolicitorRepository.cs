using SolicitorFinder.Data.DTO;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Interfaces;

public interface ISolicitorRepository : IBaseRepository<Solicitor>
{
    Task<UpsertDataResult> UpsertRangeAsync(IReadOnlyList<Solicitor> solicitors, CancellationToken cancellationToken = default);
    Task AddSolicitorRelationsAsync(IReadOnlyList<int> solicitorIds, int locationId, int areaId, CancellationToken cancellationToken = default);
}
