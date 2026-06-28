using Microsoft.AspNetCore.Mvc;
using SolicitorFinder.Application.Area;
using SolicitorFinder.Application.Area.Dto;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AreaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AreaController> _logger;

    public AreaController(IMediator mediator, ILogger<AreaController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<List<AreaInfo>> GetConfig()
    {
        _logger.LogInformation("GetConfig endpoint called in AreaController");
        var result = await _mediator.Send(new GetAreasQuery());
        _logger.LogInformation("GetConfig returned {Count} areas", result.Count);

        return result;
    }
}
