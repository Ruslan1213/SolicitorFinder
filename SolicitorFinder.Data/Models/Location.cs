namespace SolicitorFinder.Data.Models;

public sealed class Location : BaseEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public int? ConfigEntityId { get; set; }

    public ConfigEntity? Config { get; set; }

    public List<SolicitorLocation>? SolicitorsLocation { get; set; }
}
