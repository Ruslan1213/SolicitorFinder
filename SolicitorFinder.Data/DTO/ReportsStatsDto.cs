namespace SolicitorFinder.Data.Dtos;

public sealed class ReportsStatsDto
{
    public int TotalSolicitors { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalLocations { get; set; }
    public int TotalAreas { get; set; }
    public DateTime? LastUpdated { get; set; }
}
