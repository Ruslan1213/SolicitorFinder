namespace SolicitorFinder.Application.Location.Dtos;

public class LocationDto
{
    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;
}

public class LocationIdDto : LocationDto
{
    public int Id { get; set; }
}
