using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Location.GetLocations;

public sealed class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, List<LocationIdDto>>
{
    private readonly IConfigRepository _configRepository;

    public GetLocationsQueryHandler(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<List<LocationIdDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var config = await _configRepository.GetFirstAsync(cancellationToken);

        if (config == null || config.Locations == null)
        {
            return new List<LocationIdDto>();
        }

        return config.Locations
            .Select(x => new LocationIdDto
            {
                Id = x.Id,
                Title = x.Title,
                Text = x.Text,
            }).ToList();
    }
}
