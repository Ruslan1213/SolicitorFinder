namespace SolicitorFinder.Services.Configurations;

public sealed class ScraperConfiguration
{
    public static string Name = "Scraper";

    public string BaseUrl { get; set; } = null!;

    public string LocationBaseUrl { get; set; } = null!;

    public string UrlTemplate { get; set; } = null!;

    public string UrlTypeTemplate { get; set; } = null!;

    public string UrlOnlyTypeTemplate { get; set; } = null!;
}
