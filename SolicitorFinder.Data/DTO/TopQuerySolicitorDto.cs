namespace SolicitorFinder.Data.Dtos;

public sealed class TopQuerySolicitorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double RatingStars { get; set; }
    public int ReviewCount { get; set; }
    public string? Location { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
}
