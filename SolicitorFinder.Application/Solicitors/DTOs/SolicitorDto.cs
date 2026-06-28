namespace SolicitorFinder.Application.Solicitors.DTOs;

public sealed class SolicitorDto
{
    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }

    public double RatingStars { get; set; }

    public int ReviewCount { get; set; }

    public string? Location { get; set; }

    public DateTime ScrapedAt { get; set; }
}
