using SolicitorFinder.Data.Dtos;
using SolicitorFinder.Data.Interfaces;

namespace SolicitorFinder.Application.Report.Services;

public sealed class ReportingService : IReportingService
{
    private readonly IReportingRepository _repository;

    public ReportingService(IReportingRepository repository)
    {
        _repository = repository;
    }

    public Task<ReportsStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
        => _repository.GetStatsAsync(cancellationToken);

    public Task<IEnumerable<RatingDistributionDto>> GetRatingDistributionAsync(
        CancellationToken cancellationToken = default)
        => _repository.GetRatingDistributionAsync(cancellationToken);

    public Task<IEnumerable<LocationStatsQueryDto>> GetLocationStatsAsync(
        CancellationToken cancellationToken = default)
        => _repository.GetLocationStatsAsync(cancellationToken);

    public Task<IEnumerable<TopQuerySolicitorDto>> GetTopRatedAsync(
        int topCount = 5,
        CancellationToken cancellationToken = default)
        => _repository.GetTopRatedAsync(topCount, cancellationToken);

    public Task<IEnumerable<TopQuerySolicitorDto>> GetTopReviewedAsync(
        int topCount = 5,
        CancellationToken cancellationToken = default)
        => _repository.GetTopReviewedAsync(topCount, cancellationToken);
}
