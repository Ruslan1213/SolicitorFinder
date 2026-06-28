namespace SolicitorFinder.Data.Models;

public sealed class AreaEntity : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SolicitorAreaExternalId { get; set; } = string.Empty;

    public List<SolicitorArea> SolicitorArea { get; set; } = new();
}
