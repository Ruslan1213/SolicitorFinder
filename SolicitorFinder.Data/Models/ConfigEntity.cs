namespace SolicitorFinder.Data.Models;

public sealed class ConfigEntity : BaseEntity
{
    public int Id { get; set; }

    public int UpdateInterval { get; set; }

    public bool AutoUpdate { get; set; }

    public int MaxResults { get; set; }

    public List<Location> Locations { get; set; } = new();
}
