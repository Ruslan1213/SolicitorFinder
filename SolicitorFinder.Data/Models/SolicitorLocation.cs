namespace SolicitorFinder.Data.Models;

public sealed class SolicitorLocation : BaseEntity
{
    public int SolicitorId { get; set; }

    public Solicitor Solicitor { get; set; } = null!;

    public int LocationId { get; set; }

    public Location Location { get; set; } = null!;
}
