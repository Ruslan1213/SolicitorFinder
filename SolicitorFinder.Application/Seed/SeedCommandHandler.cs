using Microsoft.Extensions.Options;
using SolicitorFinder.Application.Configurations;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Models;
using SolicitorFinder.Mediator.Interfaces;
using SolicitorFinder.Services.Interfaces;

namespace SolicitorFinder.Application.Seed;

public sealed class SeedCommandHandler : IRequestHandler<SeedCommand, bool>
{
    private readonly IAreaService _areaService;
    private readonly ILocationService _locationService;
    private readonly IBaseRepository<AreaEntity> _areaRepository;
    private readonly IBaseRepository<ConfigEntity> _configRepository;
    private readonly IBaseRepository<Data.Models.Location> _locationRepository;
    private readonly LocationConfig _locationConfig;
    private readonly IUnitOfWork _unitOfWork;

    public SeedCommandHandler(
        IAreaService areaService,
        ILocationService locationService,
        IBaseRepository<ConfigEntity> configRepository,
        IBaseRepository<Data.Models.Location> locationRepository,
        IOptions<LocationConfig> options,
        IBaseRepository<AreaEntity> areaRepository,
        IUnitOfWork unitOfWork)
    {
        _areaService = areaService;
        _locationService = locationService;
        _configRepository = configRepository;
        _locationRepository = locationRepository;
        _locationConfig = options.Value;
        _areaRepository = areaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SeedCommand request, CancellationToken cancellationToken)
    {
        var locationList = new List<Data.Models.Location>();
        var isAreasExist = await _areaRepository.AnyAsync(cancellationToken);
        var isLocationExist = await _locationRepository.AnyAsync(cancellationToken);
        var isConfigExist = await _configRepository.AnyAsync(cancellationToken);

        if (!isAreasExist)
        {
            var areas = await _areaService.GetAreasAsync();

            await _areaRepository.AddRangeAsync(areas.Select(x => new AreaEntity
            {
                Name = x.Name,
                SolicitorAreaExternalId = x.Id
            }), cancellationToken);
        }

        if (!isLocationExist)
        {
            foreach (var location in _locationConfig.Locations ?? Enumerable.Empty<string>())
            {
                var parsedLocation = await _locationService.SearchLocationsAsync(location, cancellationToken);

                if (parsedLocation != null)
                {
                    var locations = parsedLocation.Select(x => new Data.Models.Location
                    {
                        Title = x.Title,
                        Text = x.Text
                    });
                    locationList.Add(locations.First());
                }
            }
        }

        if (!isConfigExist)
        {
            var configEntity = new ConfigEntity
            {
                UpdateInterval = 1440,
                MaxResults = 10,
                AutoUpdate = true,
                Locations = locationList
            };

            await _configRepository.AddAsync(configEntity, cancellationToken);
        }
        else if (!isLocationExist)
        {
            await _locationRepository.AddRangeAsync(locationList, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
