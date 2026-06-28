namespace SolicitorFinder.Application.GetLocationsAndAreas.Dtos;

public sealed class ConfigGetDto
{
    public int UpdateInterval { get; set; } = 60;
    public bool AutoUpdate { get; set; } = true;
    public int MaxResults { get; set; } = 100;
}