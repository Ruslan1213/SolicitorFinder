using SolicitorFinder.GeneralParser.Enums;

namespace SolicitorFinder.GeneralParser.Models;

public sealed class PageSelector
{
    public string Selector { get; }

    public SearchType SearchType { get; }

    public int Index { get; set; }

    public PageSelector(string selector, SearchType searchType = SearchType.First, int index = 0)
    {
        Selector = selector;
        SearchType = searchType;
        Index = index;
    }
}
