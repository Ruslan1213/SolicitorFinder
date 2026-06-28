using Microsoft.AspNetCore.Mvc;
using SolicitorFinder.Application.Config.Dtos;
using SolicitorFinder.Application.Config.GetConfig;
using SolicitorFinder.Application.Config.UpdateCommand;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ConfigurationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(IMediator mediator, ILogger<ConfigurationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ConfigDto>> GetConfig()
    {
        _logger.LogInformation("GetConfig endpoint called in ConfigurationController");
        var result = await _mediator.Send(new GetConfigQuery());
        _logger.LogInformation("GetConfig successfully returned configuration");

        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<ConfigDto>> UpdateConfig([FromBody] UpdateConfigCommand command)
    {
        _logger.LogInformation("UpdateConfig endpoint called with command");
        var result = await _mediator.Send(command);
        _logger.LogInformation("UpdateConfig successfully updated configuration");

        return Ok(result);
    }
}
