namespace SolicitorFinder.Application.GetLocationsAndAreas.Dtos;

public sealed class LocationGetDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
