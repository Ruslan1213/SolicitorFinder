namespace SolicitorFinder.Data.Dtos;

public sealed class LocationStatsQueryDto
{
    public string Location { get; set; } = string.Empty;
    public int SolicitorsCount { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}