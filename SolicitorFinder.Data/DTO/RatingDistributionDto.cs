namespace SolicitorFinder.Data.Dtos;

public sealed class RatingDistributionDto
{
    public string Range { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}
