namespace SolicitorFinder.GeneralParser.Models;

public sealed class ParseOptions
{
    public string? BaseUrl { get; set; }

    public List<PageSelector> Selectors { get; set; } = new();

    public Dictionary<string, string> CustomExtractors { get; set; } = new();
}
