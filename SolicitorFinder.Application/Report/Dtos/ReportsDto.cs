using SolicitorFinder.Data.Dtos;

namespace SolicitorFinder.Application.Report.Dtos;

public sealed class ReportsDto
{
    public ReportsStatsDto Stats { get; set; } = new();

    public List<TopQuerySolicitorDto> TopRated { get; set; } = new();

    public List<TopQuerySolicitorDto> TopReviewed { get; set; } = new();

    public List<RatingDistributionDto> RatingDistribution { get; set; } = new();

    public List<LocationStatsQueryDto> LocationStats { get; set; } = new();
}
