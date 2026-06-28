using SolicitorFinder.GeneralParser.Enums;
using SolicitorFinder.GeneralParser.Extensions;
using SolicitorFinder.GeneralParser.Interfaces;
using SolicitorFinder.GeneralParser.Models;
using SolicitorFinder.Services.Extensions;
using SolicitorFinder.Services.Interfaces;
using SolicitorFinder.Services.Models;
using System.Text.RegularExpressions;
using static SolicitorFinder.Services.Constants.ScraperConstants;

namespace SolicitorFinder.Services.Services;

public sealed class ScraperService : IScraperParserService
{
    private readonly HttpClient _httpClient;
    private readonly IPageParser _parser;
    private readonly IUrlBuilder _urlBuilder;

    public ScraperService(
        HttpClient httpClient,
        IPageParser pageParser,
        IUrlBuilder urlBuilder)
    {
        _httpClient = httpClient;
        _parser = pageParser;
        _httpClient.DefaultRequestHeaders.Add(UserAgentHeader, UserAgent);
        _urlBuilder = urlBuilder;
    }

    public async Task<List<Solicitor>> ScrapeSolicitorsAsync(SolicitorSearchModel solicitorSearchModel)
    {
        var solicitors = new List<Solicitor>();
        var url = _urlBuilder.BuildUrl(solicitorSearchModel);

        string html;
        try
        {
            html = await _httpClient.GetStringAsync(url);
        }
        catch (Exception)
        {
            return solicitors;
        }

        var result = _parser.Parse(html, GetDefaultParseOptions());

        if (!result.Success)
        {
            return solicitors;
        }

        foreach (var item in result.Elements)
        {
            var solicitor = ParseSolicitorFromElement(item, solicitorSearchModel.Location!);

            if (solicitor != null)
            {
                solicitors.Add(solicitor);
            }
        }

        return solicitors;
    }

    private Solicitor? ParseSolicitorFromElement(PageElement element, string location)
    {
        var nameElement = element.Children?
            .FirstOrDefault(c => c.TagName == Html.DivTag)?
            .Children?
            .FirstOrDefault(c => c.TagName == Html.SpanTag)
            ?? element.Children?.FirstOrDefault();

        if (nameElement == null)
        {
            return null;
        }

        var fullName = nameElement.InnerText ?? Defaults.UnknownName;
        var name = ExtractName(fullName).CleanName();

        return new Solicitor
        {
            Name = name,
            Phone = ExtractPhone(element).CleanPhone().Trim(),
            Address = ExtractAddress(element).Clean(),
            Description = ExtractDescription(element)?.Clean() ?? string.Empty,
            Website = ExtractWebsite(element)?.CleanUrl() ?? string.Empty,
            RatingStars = ExtractRatingStars(element),
            ReviewCount = ExtractReviewCount(element),
            Location = location,
            ScrapedAt = DateTime.UtcNow
        };
    }

    private string ExtractName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            return Defaults.UnknownName;
        }

        var index = fullName.IndexOf('(');

        if (index != -1)
        {
            return fullName.Substring(0, index).Trim();
        }

        return fullName.Trim();
    }

    private string ExtractPhone(PageElement element)
    {
        var phoneElement = element
            .Children?
            .FirstOrDefault()?
            .Children?
            .FirstOrDefault(x => x.ClassName == CssClasses.PhoneBlock)?
            .FindFirst(Html.ATag);

        var phone = phoneElement?.InnerText ??
            element?.Children?.LastOrDefault(x => x.TagName == Html.ATag)?.InnerText ??
            element?.Children?.LastOrDefault()?.InnerHtml;

        return phone?.Trim() ?? string.Empty;
    }

    private string ExtractAddress(PageElement element)
    {
        return element?.Children?
            .FirstOrDefault(x => x.TagName == Html.ATag)?
            .InnerText ?? string.Empty;
    }

    private string? ExtractDescription(PageElement element)
    {
        var descElement = element.FindFirst(Html.PTag);

        if (descElement != null)
        {
            var text = descElement.InnerText?.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
        }

        return null;
    }

    private string? ExtractWebsite(PageElement element)
    {
        var linkHtml = element.
            Children?
            .FirstOrDefault(x => x.TagName == Html.UlTag)?
            .Children?
            .FirstOrDefault(x => string.IsNullOrEmpty(x.ClassName))?
            .InnerHtml;

        return FindFirstHref(linkHtml);
    }

    private double ExtractRatingStars(PageElement element)
    {
        var fullStars = element.FindAll(Selectors.StarFull).Count;
        var halfStars = element.FindAll(Selectors.StarHalf).Count;

        if (fullStars == Defaults.Zero && halfStars == Defaults.Zero)
        {
            fullStars = element.Children.Count(c => c.ClassName?.Contains(CssClasses.StarFull) == true);
            halfStars = element.Children.Count(c => c.ClassName?.Contains(CssClasses.StarHalf) == true);
        }

        return fullStars + halfStars * 0.5;
    }

    private int ExtractReviewCount(PageElement element)
    {
        var reviewChild = element.Children.FirstOrDefault()?.Children?
            .FirstOrDefault(c => c.TagName?.Contains(Html.SpanTag) == true);

        if (reviewChild?.InnerText != null)
        {
            var match = Regex.Match(reviewChild.InnerText, RegexPatterns.ReviewCount);

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
        }

        return Defaults.Zero;
    }

    private static string? FindFirstHref(string? html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return null;
        }

        var match = Regex.Match(html, Html.HrefPattern);

        return match.Success ? match.Groups[1].Value : null;
    }

    private static ParseOptions GetDefaultParseOptions()
    {
        return new ParseOptions
        {
            BaseUrl = BaseUrl,
            Selectors = new List<PageSelector>
            {
                new PageSelector(Selectors.ResultItem, SearchType.All),
                new PageSelector(Selectors.ResultItemH2, SearchType.First),
                new PageSelector(Selectors.PhoneBlockLink, SearchType.First),
                new PageSelector(Selectors.LinkMapAddress, SearchType.First),
                new PageSelector(Selectors.ResultItemP, SearchType.First)
            }
        };
    }
}