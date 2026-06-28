using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Interfaces;

public interface IConfigRepository : IBaseRepository<ConfigEntity>
{
    Task<ConfigEntity?> GetFirstAsync(CancellationToken cancellationToken);
}
