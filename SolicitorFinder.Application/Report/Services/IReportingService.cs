using SolicitorFinder.Data.Dtos;

namespace SolicitorFinder.Application.Report.Services;

public interface IReportingService
{
    Task<ReportsStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<RatingDistributionDto>> GetRatingDistributionAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<LocationStatsQueryDto>> GetLocationStatsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TopQuerySolicitorDto>> GetTopRatedAsync(int topCount = 5, CancellationToken cancellationToken = default);

    Task<IEnumerable<TopQuerySolicitorDto>> GetTopReviewedAsync(int topCount = 5, CancellationToken cancellationToken = default);
}
