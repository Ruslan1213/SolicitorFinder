using SolicitorFinder.Application.GetLocationsAndAreas;
using SolicitorFinder.Application.Solicitors.Commands.ScrapeSolicitors;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Common.BackgroundServices;

public sealed class SolicitorScraperService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SolicitorScraperService> _logger;
    private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(10);

    public SolicitorScraperService(
        IServiceProvider serviceProvider,
        ILogger<SolicitorScraperService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int interval = 1440;
        _logger.LogInformation("SolicitorScraperService started");

        await Task.Delay(_initialDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                interval = await RunScrapeAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in scrape cycle");
            }

            _logger.LogInformation($"Next scrape in {interval} minutes");

            await Task.Delay(interval * 60 * 1000, stoppingToken);
        }

        _logger.LogInformation("SolicitorScraperService stopped");
    }

    private async Task<int> RunScrapeAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        _logger.LogInformation("Starting scrape cycle...");

        var configQuery = new GetLocationsAndAreasQuery();
        var data = await mediator.Send(configQuery, cancellationToken);

        if (!data.Config.AutoUpdate)
        {
            _logger.LogInformation("AutoUpdate is disabled, skipping scrape");

            return data.Config.UpdateInterval;
        }

        var result = await mediator.Send(new ScrapeSolicitorsCommand(), cancellationToken);
        _logger.LogInformation(
            "Scrape completed: Added={Added}, Updated={Updated}, Skipped={Skipped}, Total={Total}, Errors={Errors}",
            result.Added,
            result.Updated,
            result.Skipped,
            result.Total,
            result.Errors.Count);

        return data.Config.UpdateInterval;
    }
}