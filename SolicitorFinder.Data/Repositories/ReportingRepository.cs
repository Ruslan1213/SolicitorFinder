using Microsoft.EntityFrameworkCore;
using SolicitorFinder.Data.Dtos;
using SolicitorFinder.Data.Interfaces;

namespace SolicitorFinder.Data.Repositories;

public sealed class ReportingRepository : IReportingRepository
{
    private readonly SolicitorDbContext _context;
    private static readonly string[] RatingRanges = { "0-1", "1-2", "2-3", "3-4", "4-5" };

    public ReportingRepository(SolicitorDbContext context)
    {
        _context = context;
    }

    public async Task<ReportsStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _context.Solicitors
            .GroupBy(s => 1)
            .Select(g => new ReportsStatsDto
            {
                TotalSolicitors = g.Count(),
                AverageRating = g.Average(s => s.RatingStars),
                TotalReviews = g.Sum(s => s.ReviewCount),
                TotalLocations = g.SelectMany(s => s.SolicitorLocations).Select(l => l.LocationId).Distinct().Count(),
                TotalAreas = g.SelectMany(s => s.SolicitorAreas).Select(a => a.AreaId).Distinct().Count(),
                LastUpdated = g.Max(s => s.ScrapedAt)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return stats ?? new ReportsStatsDto();
    }

    public async Task<IEnumerable<RatingDistributionDto>> GetRatingDistributionAsync(
        CancellationToken cancellationToken = default)
    {
        var distribution = await _context.Solicitors
            .GroupBy(s => s.RatingStars >= 4 ? "4-5"
                        : s.RatingStars >= 3 ? "3-4"
                        : s.RatingStars >= 2 ? "2-3"
                        : s.RatingStars >= 1 ? "1-2" : "0-1")
            .Select(g => new { Range = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var total = distribution.Sum(d => d.Count);
        var distDict = distribution.ToDictionary(d => d.Range, d => d.Count);

        return RatingRanges.Select(range => new RatingDistributionDto
        {
            Range = range,
            Count = distDict.GetValueOrDefault(range, 0),
            Percentage = total > 0
                ? Math.Round((double)distDict.GetValueOrDefault(range, 0) / total * 100, 1)
                : 0
        }).ToList();
    }

    public async Task<IEnumerable<LocationStatsQueryDto>> GetLocationStatsAsync(
        CancellationToken cancellationToken = default)
    {
        var raw = await _context.SolicitorLocations
            .GroupBy(sl => sl.Location!.Title)
            .Select(g => new
            {
                Location = g.Key,
                SolicitorsCount = g.Count(),
                AverageRating = g.Average(sl => sl.Solicitor!.RatingStars),
                TotalReviews = g.Sum(sl => sl.Solicitor!.ReviewCount)
            })
            .OrderByDescending(l => l.SolicitorsCount)
            .ToListAsync(cancellationToken);

        return raw.Select(x => new LocationStatsQueryDto
        {
            Location = x.Location ?? string.Empty,
            SolicitorsCount = x.SolicitorsCount,
            AverageRating = Math.Round(x.AverageRating, 1),
            TotalReviews = x.TotalReviews
        });
    }

    public async Task<IEnumerable<TopQuerySolicitorDto>> GetTopRatedAsync(
        int topCount = 5,
        CancellationToken cancellationToken = default)
    {
        return await _context.Solicitors
            .OrderByDescending(s => s.RatingStars)
            .ThenByDescending(s => s.ReviewCount)
            .Take(topCount)
            .Select(s => new TopQuerySolicitorDto
            {
                Id = s.Id,
                Name = s.Name!,
                RatingStars = s.RatingStars,
                ReviewCount = s.ReviewCount,
                Location = s.SolicitorLocations.Select(l => l.Location!.Title).FirstOrDefault() ?? string.Empty,
                Address = s.Address,
                Phone = s.Phone,
                Website = s.Website
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TopQuerySolicitorDto>> GetTopReviewedAsync(
        int topCount = 5,
        CancellationToken cancellationToken = default)
    {
        return await _context.Solicitors
            .OrderByDescending(s => s.ReviewCount)
            .Take(topCount)
            .Select(s => new TopQuerySolicitorDto
            {
                Id = s.Id,
                Name = s.Name!,
                RatingStars = s.RatingStars,
                ReviewCount = s.ReviewCount,
                Location = s.SolicitorLocations.Select(l => l.Location!.Title).FirstOrDefault() ?? string.Empty,
                Address = s.Address,
                Phone = s.Phone,
                Website = s.Website
            })
            .ToListAsync(cancellationToken);
    }
}
