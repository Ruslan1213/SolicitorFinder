namespace SolicitorFinder.Application.ScrapeSolicitors.Dtos;

public sealed class ScrapeResultDto
{
    public int Added { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public int Total { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public List<string> Errors { get; set; } = new();
}