using SolicitorFinder.GeneralParser.Interfaces;
using SolicitorFinder.GeneralParser.Models;

namespace SolicitorFinder.GeneralParser.Core;

public class BaseParser : IPageParser
{
    protected readonly IHtmlParser HtmlParser;

    public BaseParser(IHtmlParser htmlParser)
    {
        HtmlParser = htmlParser;
    }

    public virtual ParseResult Parse(string html, ParseOptions options)
    {
        var result = new ParseResult
        {
            Success = true,
            ParsedAt = DateTime.UtcNow
        };

        try
        {
            result.Title = HtmlParser.ExtractText(html, "title");

            foreach (var selector in options.Selectors)
            {
                var elements = HtmlParser.ParseElements(html, selector.Selector, selector.SearchType);
                result.Elements.AddRange(elements);
            }

            foreach (var extractor in options.CustomExtractors)
            {
                var value = HtmlParser.ExtractText(html, extractor.Value);
                result.SetData(extractor.Key, value ?? string.Empty);
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public virtual List<PageElement> ExtractElements(string html, PageSelector selector)
    {
        return HtmlParser.ParseElements(html, selector.Selector, selector.SearchType);
    }

    public virtual string? ExtractText(string html, PageSelector selector)
    {
        return HtmlParser.ExtractText(html, selector.Selector);
    }

    public virtual string? ExtractAttribute(string html, PageSelector selector, string attribute)
    {
        return HtmlParser.ExtractAttribute(html, selector.Selector, attribute);
    }
}
