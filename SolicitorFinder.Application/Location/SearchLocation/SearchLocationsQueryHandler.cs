using Microsoft.Extensions.Logging;
using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Mediator.Interfaces;
using SolicitorFinder.Services.Interfaces;

namespace SolicitorFinder.Application.Location.SearchLocation;
public sealed class SearchLocationsQueryHandler : IRequestHandler<SearchLocationsQuery, List<LocationDto>>
{
    private readonly ILocationService _locationService;
    private readonly ILogger<SearchLocationsQueryHandler> _logger;

    public SearchLocationsQueryHandler(ILocationService locationService, ILogger<SearchLocationsQueryHandler> logger)
    {
        _locationService = locationService;
        _logger = logger;
    }

    public async Task<List<LocationDto>> Handle(SearchLocationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SearchLocationsQuery with SearchText: {SearchText}", request.SearchText);

        var locations = await _locationService.SearchLocationsAsync(request.SearchText, cancellationToken);

        var result = locations.Select(l => new LocationDto
        {
            Title = l.Title,
            Text = l.Text
        }).ToList();

        _logger.LogInformation("Returning {Count} locations", result.Count);
        return result;
    }
}
