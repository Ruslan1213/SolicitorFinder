using Microsoft.Extensions.Logging;
using SolicitorFinder.Application.Report.Dtos;
using SolicitorFinder.Application.Report.Services;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Report;

public sealed class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, ReportsDto>
{
    private readonly IReportingService _reportingService;
    private readonly ILogger<GetReportsQueryHandler> _logger;

    public GetReportsQueryHandler(
        IReportingService reportingService,
        ILogger<GetReportsQueryHandler> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    public async Task<ReportsDto> Handle(GetReportsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var topCount = request.TopCount ?? 5;

            var stats = await _reportingService.GetStatsAsync(cancellationToken);
            var topRated = await _reportingService.GetTopRatedAsync(topCount, cancellationToken);
            var topReviewed = await _reportingService.GetTopReviewedAsync(topCount, cancellationToken);
            var distribution = await _reportingService.GetRatingDistributionAsync(cancellationToken);
            var locationStats = await _reportingService.GetLocationStatsAsync(cancellationToken);

            return new ReportsDto
            {
                Stats = stats,
                TopRated = topRated.ToList(),
                TopReviewed = topReviewed.ToList(),
                RatingDistribution = distribution.ToList(),
                LocationStats = locationStats.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating reports");
            throw;
        }
    }
}
