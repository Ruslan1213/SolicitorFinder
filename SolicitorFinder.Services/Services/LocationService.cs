using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolicitorFinder.Services.Configurations;
using SolicitorFinder.Services.Interfaces;
using SolicitorFinder.Services.Models;
using System.Text.Json;
using static SolicitorFinder.Services.Constants.ScraperConstants;


namespace SolicitorFinder.Services.Services;

public sealed class LocationService : ILocationService
{
    private readonly HttpClient _httpClient;
    private readonly ScraperConfiguration _config;
    private readonly ILogger<LocationService> _logger;

    public LocationService(HttpClient httpClient, IOptions<ScraperConfiguration> config, ILogger<LocationService> logger)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add(UserAgentHeader, UserAgent);
        _config = config.Value;
        _logger = logger;
    }

    public async Task<List<LocationResponse>> SearchLocationsAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SearchLocationsAsync called with query: {Query}", query);

        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            _logger.LogWarning("Query is invalid or too short: {Query}", query);

            return new List<LocationResponse>();
        }

        try
        {
            var url = string.Format(_config.LocationBaseUrl, Uri.EscapeDataString(query));
            _logger.LogDebug("Fetching locations from URL: {Url}", url);

            var response = await _httpClient.GetStringAsync(url);
            var locations = JsonSerializer.Deserialize<List<LocationResponse>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Successfully retrieved {Count} locations for query: {Query}", locations?.Count ?? 0, query);

            return locations ?? new List<LocationResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching locations for query: {Query}", query);

            return new List<LocationResponse>();
        }
    }
}
