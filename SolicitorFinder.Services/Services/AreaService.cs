using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolicitorFinder.GeneralParser.Enums;
using SolicitorFinder.GeneralParser.Interfaces;
using SolicitorFinder.GeneralParser.Models;
using SolicitorFinder.Services.Configurations;
using SolicitorFinder.Services.Interfaces;
using SolicitorFinder.Services.Models;
using static SolicitorFinder.Services.Constants.ScraperConstants;

namespace SolicitorFinder.Services.Services;

public sealed class AreaService : IAreaService
{
    private readonly HttpClient _httpClient;
    private readonly IPageParser _parser;
    private readonly ScraperConfiguration _scraperConfiguration;
    private readonly ILogger<AreaService> _logger;

    public AreaService(
        HttpClient httpClient,
        IPageParser pageParser,
        IOptions<ScraperConfiguration> options,
        ILogger<AreaService> logger)
    {
        _httpClient = httpClient;
        _parser = pageParser;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add(UserAgentHeader, UserAgent);
        _scraperConfiguration = options.Value;
        _logger = logger;
    }

    public async Task<List<AreaDto>> GetAreasAsync()
    {
        _logger.LogInformation("GetAreasAsync started");

        var areas = new List<AreaDto>();
        var url = _scraperConfiguration.BaseUrl;

        _logger.LogDebug("Parsing URL: {Url}", url);

        string html;

        try
        {
            html = await _httpClient.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "HTTP request failed for URL: {Url}", url);
            return areas;
        }

        var result = _parser.Parse(html, GetDefaultParseOptions());

        if (!result.Success)
        {
            _logger.LogWarning("Parser failed to retrieve data from URL: {Url}", url);
            return areas;
        }

        var selectElement = result.Elements.FirstOrDefault();

        if (selectElement == null)
        {
            _logger.LogWarning("No select element found in parsed result");
            return areas;
        }

        var optionElements = selectElement.Children
            .Where(c => c.TagName == Html.OptionsTag && c.Attributes?.Any() == true)
            .Select(x => new AreaDto
            {
                Id = x.Attributes.First().Value,
                Name = x.InnerText ?? string.Empty
            })
            .ToList();

        _logger.LogInformation("Successfully retrieved {Count} areas", optionElements.Count);

        return optionElements;
    }

    private static ParseOptions GetDefaultParseOptions()
    {
        return new ParseOptions
        {
            BaseUrl = BaseUrl,
            Selectors = new List<PageSelector>
            {
                new PageSelector(Selectors.SelectClass, SearchType.First),
                new PageSelector(Html.SelectTag, SearchType.All)
            }
        };
    }
}
