using SolicitorFinder.GeneralParser.Models;

namespace SolicitorFinder.GeneralParser.Interfaces;

public interface IPageParser
{
    ParseResult Parse(string html, ParseOptions options);

    List<PageElement> ExtractElements(string html, PageSelector selector);

    string? ExtractText(string html, PageSelector selector);

    string? ExtractAttribute(string html, PageSelector selector, string attribute);
}
