using SolicitorFinder.Application.Location.Dtos;

namespace SolicitorFinder.Application.Config.Dtos;

public sealed class ConfigDto
{
    public int UpdateInterval { get; set; }

    public bool AutoUpdate { get; set; }

    public int MaxResults { get; set; }

    public List<LocationDto> Locations { get; set; } = new();
}