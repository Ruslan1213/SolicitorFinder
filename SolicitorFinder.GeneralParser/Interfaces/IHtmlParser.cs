using SolicitorFinder.GeneralParser.Enums;
using SolicitorFinder.GeneralParser.Models;

namespace SolicitorFinder.GeneralParser.Interfaces;

public interface IHtmlParser
{
    string? ExtractAttribute(string html, string selector, string attribute);

    string? ExtractText(string html, string selector);

    List<PageElement> ParseElements(string html, string selector, SearchType searchType = SearchType.All);
}