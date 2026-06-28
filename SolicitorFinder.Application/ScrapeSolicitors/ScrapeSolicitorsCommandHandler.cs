using Microsoft.Extensions.Logging;
using SolicitorFinder.Application.ScrapeSolicitors.Dtos;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Models;
using SolicitorFinder.Mediator.Interfaces;
using SolicitorFinder.Services.Interfaces;
using SolicitorFinder.Services.Models;

namespace SolicitorFinder.Application.Solicitors.Commands.ScrapeSolicitors;
public sealed class ScrapeSolicitorsCommandHandler : IRequestHandler<ScrapeSolicitorsCommand, ScrapeResultDto>
{
    private const int BatchSize = 50;

    private readonly ISolicitorRepository _solicitorRepository;
    private readonly IBaseRepository<Data.Models.Location> _locationRepository;
    private readonly IBaseRepository<AreaEntity> _areaRepository;
    private readonly IScraperParserService _parserService;
    private readonly ILogger<ScrapeSolicitorsCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ScrapeSolicitorsCommandHandler(
        ISolicitorRepository solicitorRepository,
        IBaseRepository<Data.Models.Location> locationRepository,
        IBaseRepository<AreaEntity> areaRepository,
        IScraperParserService parserService,
        ILogger<ScrapeSolicitorsCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _solicitorRepository = solicitorRepository;
        _locationRepository = locationRepository;
        _areaRepository = areaRepository;
        _parserService = parserService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ScrapeResultDto> Handle(ScrapeSolicitorsCommand request, CancellationToken cancellationToken)
    {
        var result = new ScrapeResultDto { StartedAt = DateTime.UtcNow };

        try
        {
            var locations = await _locationRepository.GetAllAsync(cancellationToken);
            var areas = await _areaRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Starting scrape for {LocationCount} locations and {AreaCount} areas",
                locations.Count, areas.Count);

            foreach (var location in locations)
            {
                foreach (var area in areas)
                {
                    await ScrapeLocationArea(location, area, result, cancellationToken);
                }
            }

            result.CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during scrape");
            result.Errors.Add(ex.Message);
        }

        return result;
    }

    private async Task ScrapeLocationArea(
        Data.Models.Location location,
        AreaEntity area,
        ScrapeResultDto result,
        CancellationToken cancellationToken)
    {
        try
        {
            int added = 0, updated = 0, total = 0;
            var solicitors = await GetParseSolicitorsAsync(location, area, cancellationToken);

            if (!solicitors.Any())
            {
                return;
            }

            _logger.LogDebug("Found {Count} solicitors for {Location} - {Area}",
                solicitors.Count, location.Title, area.Name);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                foreach (var batch in solicitors.Chunk(BatchSize))
                {
                    var upsertDataResult = await _solicitorRepository.UpsertRangeAsync(batch, cancellationToken);
                    await _solicitorRepository.AddSolicitorRelationsAsync(upsertDataResult.Ids, location.Id, area.Id, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _unitOfWork.ClearChangeTracker();

                    added += upsertDataResult.Added;
                    updated += upsertDataResult.Updated;
                    total += batch.Length;
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                result.Added += added;
                result.Updated += updated;
                result.Total += total;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scraping solicitors for {Location} - {Area}", location.Title, area.Name);
            result.Errors.Add($"Error scraping solicitors for {location.Title} - {area.Name}: {ex.Message}");
            result.Skipped++;
        }
    }

    private async Task<List<Data.Models.Solicitor>> GetParseSolicitorsAsync(
        Data.Models.Location location,
        AreaEntity area,
        CancellationToken cancellationToken)
    {
        return (await _parserService.ScrapeSolicitorsAsync(new SolicitorSearchModel
        {
            AreaId = area.SolicitorAreaExternalId,
            Location = location.Title.ToLower(),
        })).GroupBy(s => s.Name)
                .Select(g =>
                {
                    var primary = g.OrderByDescending(x => x.RatingStars)
                                   .ThenByDescending(x => x.ReviewCount)
                                   .First();
                    return new Data.Models.Solicitor
                    {
                        Name = g.Key,
                        Phone = primary.Phone,
                        Address = primary.Address,
                        Description = primary.Description,
                        Website = primary.Website,
                        RatingStars = primary.RatingStars,
                        ReviewCount = primary.ReviewCount,
                        ScrapedAt = DateTime.UtcNow
                    };
                })
                .ToList();
    }
}
