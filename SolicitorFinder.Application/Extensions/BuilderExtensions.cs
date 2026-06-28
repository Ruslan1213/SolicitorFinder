using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolicitorFinder.Application;
using SolicitorFinder.Application.HostedServices;
using SolicitorFinder.Application.Area;
using SolicitorFinder.Application.Area.Dto;
using SolicitorFinder.Application.Area.SyncArea;
using SolicitorFinder.Application.Config.Dtos;
using SolicitorFinder.Application.Config.GetConfig;
using SolicitorFinder.Application.Config.UpdateCommand;
using SolicitorFinder.Application.Configurations;
using SolicitorFinder.Application.GetLocationsAndAreas;
using SolicitorFinder.Application.GetLocationsAndAreas.Dtos;
using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Application.Location.GetLocations;
using SolicitorFinder.Application.Location.InitLocations;
using SolicitorFinder.Application.Location.SearchLocation;
using SolicitorFinder.Application.Report;
using SolicitorFinder.Application.Report.Dtos;
using SolicitorFinder.Application.Report.Services;
using SolicitorFinder.Application.ScrapeSolicitors.Dtos;
using SolicitorFinder.Application.Seed;
using SolicitorFinder.Application.Solicitors.Commands.ScrapeSolicitors;
using SolicitorFinder.Application.Solicitors.DTOs;
using SolicitorFinder.Application.Solicitors.Queries.GetLocationsAndAreas;
using SolicitorFinder.Application.Solicitors.SearchSolicitor;
using SolicitorFinder.Mediator.Interfaces;
using System.Reflection;

namespace SolicitorFinder.Mediator.Extensions;

public static class BuilderExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<DatabaseSeedService>();

        services.Configure<LocationConfig>(
            configuration.GetSection(nameof(LocationConfig)));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<IRequestHandler<SearchSolicitorQuery, PagedResult<SolicitorDto>>, SearchSolicitorsQueryHandler>();
        services.AddScoped<IRequestHandler<UpdateConfigCommand, ConfigDto>, UpdateConfigCommandHandler>();
        services.AddScoped<IRequestHandler<GetConfigQuery, ConfigDto>, GetConfigQueryHandler>();
        services.AddScoped<IRequestHandler<SearchLocationsQuery, List<LocationDto>>, SearchLocationsQueryHandler>();
        services.AddScoped<IRequestHandler<GetLocationsQuery, List<LocationIdDto>>, GetLocationsQueryHandler>();
        services.AddScoped<IRequestHandler<SyncAreaCommand, bool>, SyncAreaCommandHandler>();
        services.AddScoped<IRequestHandler<InitLocationsCommand, bool>, InitLocationsCommandHandler>();
        services.AddScoped<IRequestHandler<GetAreasQuery, List<AreaInfo>>, GetAreasQueryHandler>();
        services.AddScoped<IRequestHandler<SeedCommand, bool>, SeedCommandHandler>();
        services.AddScoped<IRequestHandler<ScrapeSolicitorsCommand, ScrapeResultDto>, ScrapeSolicitorsCommandHandler>();
        services.AddScoped<IRequestHandler<GetLocationsAndAreasQuery, LocationsAndAreasDto>, GetLocationsAndAreasQueryHandler>();
        services.AddScoped<IRequestHandler<GetReportsQuery, ReportsDto>, GetReportsQueryHandler>();


        return services;
    }
}
