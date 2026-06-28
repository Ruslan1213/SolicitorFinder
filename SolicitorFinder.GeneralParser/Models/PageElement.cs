namespace SolicitorFinder.GeneralParser.Models;

public sealed class PageElement
{
    public string? TagName { get; set; }

    public string? Id { get; set; }

    public string? ClassName { get; set; }

    public string? InnerHtml { get; set; }

    public string? InnerText { get; set; }

    public Dictionary<string, string> Attributes { get; set; } = new();

    public List<PageElement> Children { get; set; } = new();

    public PageElement? Parent { get; set; }
}
