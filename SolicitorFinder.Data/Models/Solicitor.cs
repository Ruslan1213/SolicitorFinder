namespace SolicitorFinder.Data.Models;

public sealed class Solicitor : BaseEntity
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }

    public double RatingStars { get; set; }

    public int ReviewCount { get; set; }

    public DateTime ScrapedAt { get; set; }

    public List<SolicitorLocation> SolicitorLocations { get; set; } = new List<SolicitorLocation>();

    public List<SolicitorArea> SolicitorAreas { get; set; } = new List<SolicitorArea>();
}
