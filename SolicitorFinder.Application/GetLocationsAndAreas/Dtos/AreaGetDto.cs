namespace SolicitorFinder.Application.GetLocationsAndAreas.Dtos;

public sealed class AreaGetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SolicitorAreaExternalId { get; set; } = string.Empty;
}
