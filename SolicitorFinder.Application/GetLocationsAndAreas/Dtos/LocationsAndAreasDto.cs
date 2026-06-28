namespace SolicitorFinder.Application.GetLocationsAndAreas.Dtos;

public sealed class LocationsAndAreasDto
{
    public List<LocationGetDto> Locations { get; set; } = new();
    public List<AreaGetDto> Areas { get; set; } = new();
    public ConfigGetDto Config { get; set; } = new();
}
