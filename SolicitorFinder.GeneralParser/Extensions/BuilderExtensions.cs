using Microsoft.Extensions.DependencyInjection;
using SolicitorFinder.GeneralParser.Core;
using SolicitorFinder.GeneralParser.Interfaces;

namespace SolicitorFinder.GeneralParser.Extensions;

public static class BuilderExtensions
{
    public static IServiceCollection AddParserServices(this IServiceCollection services)
    {
        services.AddScoped<IHtmlParser, HtmlParser>();
        services.AddScoped<IPageParser, BaseParser>();

        return services;
    }
}
