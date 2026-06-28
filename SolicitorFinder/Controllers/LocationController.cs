using Microsoft.AspNetCore.Mvc;
using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Application.Location.GetLocations;
using SolicitorFinder.Application.Location.SearchLocation;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LocationController
{
    private readonly IMediator _mediator;
    private readonly ILogger<LocationController> _logger;

    public LocationController(IMediator mediator, ILogger<LocationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("search")]
    public async Task<List<LocationDto>> SearchLocations([FromQuery] string searchText)
    {
        _logger.LogInformation("SearchLocations endpoint called with searchText: {SearchText}", searchText);
        var result = await _mediator.Send(new SearchLocationsQuery(searchText));
        _logger.LogInformation("SearchLocations returned {Count} results", result.Count);

        return result;
    }

    [HttpGet("locations")]
    public async Task<List<LocationIdDto>> GetLocations()
    {
        _logger.LogInformation("GetLocations endpoint called");
        var result = await _mediator.Send(new GetLocationsQuery());
        _logger.LogInformation("GetLocations returned {Count} results", result.Count);

        return result;
    }
}
