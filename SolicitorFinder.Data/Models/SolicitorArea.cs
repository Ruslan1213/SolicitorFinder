namespace SolicitorFinder.Data.Models;

public sealed class SolicitorArea : BaseEntity
{
    public int SolicitorId { get; set; }

    public Solicitor Solicitor { get; set; } = null!;

    public int AreaId { get; set; }

    public AreaEntity Area { get; set; } = null!;
}
