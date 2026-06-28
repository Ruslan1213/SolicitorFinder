using Microsoft.Extensions.DependencyInjection;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Mediator.Extensions;

public static class BuilderExtensions
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        return services;
    }
}
