using SolicitorFinder.Application.GetLocationsAndAreas;
using SolicitorFinder.Application.GetLocationsAndAreas.Dtos;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Models;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Solicitors.Queries.GetLocationsAndAreas;

public sealed class GetLocationsAndAreasQueryHandler : IRequestHandler<GetLocationsAndAreasQuery, LocationsAndAreasDto>
{
    private readonly IReadRepository<Data.Models.Location> _locationRepository;
    private readonly IReadRepository<AreaEntity> _areaRepository;
    private readonly IConfigRepository _configRepository;

    public GetLocationsAndAreasQueryHandler(
        IReadRepository<Data.Models.Location> locationRepository,
        IReadRepository<AreaEntity> areaRepository,
        IConfigRepository configRepository)
    {
        _locationRepository = locationRepository;
        _areaRepository = areaRepository;
        _configRepository = configRepository;
    }

    public async Task<LocationsAndAreasDto> Handle(GetLocationsAndAreasQuery request, CancellationToken cancellationToken)
    {
        var locations = await _locationRepository.GetAllAsync(cancellationToken);
        var areas = await _areaRepository.GetAllAsync(cancellationToken);
        var config = await _configRepository.GetFirstAsync(cancellationToken);

        if (config == null)
        {
            throw new InvalidOperationException("Config not found in the database.");
        }

        return new LocationsAndAreasDto
        {
            Locations = locations.Select(l => new LocationGetDto
            {
                Id = l.Id,
                Title = l.Title,
                Text = l.Text
            }).ToList(),
            Areas = areas.Select(a => new AreaGetDto
            {
                Id = a.Id,
                Name = a.Name,
                SolicitorAreaExternalId = a.SolicitorAreaExternalId
            }).ToList(),
            Config = new ConfigGetDto
            {
                UpdateInterval = config.UpdateInterval,
                AutoUpdate = config.AutoUpdate,
                MaxResults = config.MaxResults
            }
        };
    }
}