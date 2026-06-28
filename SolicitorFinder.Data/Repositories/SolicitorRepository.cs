using Microsoft.EntityFrameworkCore;
using SolicitorFinder.Data.DTO;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Models;
using System.Linq.Expressions;

namespace SolicitorFinder.Data.Repositories;
public sealed class SolicitorRepository : BaseRepository<Solicitor>, ISolicitorRepository
{
    public SolicitorRepository(SolicitorDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Solicitor>> GetFilteredAsync(
        Expression<Func<Solicitor, bool>>? predicate = null,
        Func<IQueryable<Solicitor>, IOrderedQueryable<Solicitor>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Solicitor> query = _dbSet
            .Include(s => s.SolicitorLocations).ThenInclude(sl => sl.Location)
            .Include(s => s.SolicitorAreas).ThenInclude(sa => sa.Area);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<UpsertDataResult> UpsertRangeAsync(
        IReadOnlyList<Solicitor> solicitors,
        CancellationToken cancellationToken = default)
    {
        var names = solicitors.Select(s => s.Name).ToList();

        var existingByName = await _context.Solicitors
            .Where(s => names.Contains(s.Name!))
            .ToDictionaryAsync(s => s.Name!, cancellationToken);

        var toAdd = new List<Solicitor>();

        foreach (var solicitor in solicitors)
        {
            if (existingByName.TryGetValue(solicitor.Name!, out var existing))
            {
                existing.Phone = solicitor.Phone;
                existing.Address = solicitor.Address;
                existing.Description = solicitor.Description;
                existing.Website = solicitor.Website;
                existing.RatingStars = solicitor.RatingStars;
                existing.ReviewCount = solicitor.ReviewCount;
                existing.ScrapedAt = solicitor.ScrapedAt;
            }
            else
            {
                toAdd.Add(solicitor);
            }
        }

        if (toAdd.Any())
            await _context.Solicitors.AddRangeAsync(toAdd, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var ids = existingByName.Values.Select(s => s.Id)
            .Concat(toAdd.Select(s => s.Id))
            .ToList();

        return new UpsertDataResult(ids, toAdd.Count, existingByName.Count);
    }

    public async Task AddSolicitorRelationsAsync(
        IReadOnlyList<int> solicitorIds,
        int locationId,
        int areaId,
        CancellationToken cancellationToken = default)
    {
        var existingLocationRelations = (await _context.SolicitorLocations
            .Where(sl => sl.LocationId == locationId && solicitorIds.Contains(sl.SolicitorId))
            .Select(sl => sl.SolicitorId)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        var newLocationRelations = solicitorIds
            .Where(id => !existingLocationRelations.Contains(id))
            .Select(id => new SolicitorLocation { SolicitorId = id, LocationId = locationId })
            .ToList();

        if (newLocationRelations.Any())
        {
            await _context.SolicitorLocations.AddRangeAsync(newLocationRelations, cancellationToken);
        }

        var existingAreaRelations = (await _context.SolicitorAreas
            .Where(sa => sa.AreaId == areaId && solicitorIds.Contains(sa.SolicitorId))
            .Select(sa => sa.SolicitorId)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        var newAreaRelations = solicitorIds
            .Where(id => !existingAreaRelations.Contains(id))
            .Select(id => new SolicitorArea { SolicitorId = id, AreaId = areaId })
            .ToList();

        if (newAreaRelations.Any())
        {
            await _context.SolicitorAreas.AddRangeAsync(newAreaRelations, cancellationToken);
        }
    }
}
