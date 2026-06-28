using Microsoft.EntityFrameworkCore;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Repositories;
public sealed class ConfigRepository : BaseRepository<ConfigEntity>, IConfigRepository
{
    public ConfigRepository(SolicitorDbContext context) : base(context)
    {
    }

    public async Task<ConfigEntity?> GetFirstAsync(CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(x => x.Locations)
            .FirstOrDefaultAsync();
    }
}
