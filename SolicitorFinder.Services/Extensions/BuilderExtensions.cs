using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolicitorFinder.Services.Configurations;
using SolicitorFinder.Services.Helper;
using SolicitorFinder.Services.Interfaces;
using SolicitorFinder.Services.Services;

namespace SolicitorFinder.GeneralParser.Extensions;

public static class BuilderExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ScraperConfiguration>(configuration.GetSection(ScraperConfiguration.Name));

        services.AddHttpClient<IScraperParserService, ScraperService>()
            .ConfigurePrimaryHttpMessageHandler(GetHttpClientHandler)
            .ConfigureHttpClient(ConfigureHttpClient);

        services.AddHttpClient<IAreaService, AreaService>()
            .ConfigurePrimaryHttpMessageHandler(GetHttpClientHandler)
            .ConfigureHttpClient(ConfigureHttpClient);

        services.AddHttpClient<ILocationService, LocationService>()
            .ConfigurePrimaryHttpMessageHandler(GetHttpClientHandler)
            .ConfigureHttpClient(ConfigureHttpClient);

        services.AddScoped<IUrlBuilder, UrlBuilder>();

        return services;
    }

    private static HttpClientHandler GetHttpClientHandler() => new HttpClientHandler
    {
        AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
        AllowAutoRedirect = true,
        UseCookies = true
    };

    private static void ConfigureHttpClient(HttpClient client)
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    }
}
