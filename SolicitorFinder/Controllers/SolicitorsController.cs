using Microsoft.AspNetCore.Mvc;
using SolicitorFinder.Application;
using SolicitorFinder.Application.Report;
using SolicitorFinder.Application.Report.Dtos;
using SolicitorFinder.Application.Solicitors.DTOs;
using SolicitorFinder.Attributes;
using SolicitorFinder.Extensions;
using SolicitorFinder.Mediator.Interfaces;
using SolicitorFinder.Models.Requests;

namespace SolicitorFinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class SolicitorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SolicitorsController> _logger;

        public SolicitorsController(IMediator mediator, ILogger<SolicitorsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [ValidateModel]
        [HttpGet(Name = "SearchSolicitors")]
        public async Task<PagedResult<SolicitorDto>> Get(
            [FromQuery] SearchSolicitor searchSolicitor,
            CancellationToken cancellation = default)
        {
            _logger.LogInformation("SearchSolicitors endpoint called with parameters: {@SearchParams}", searchSolicitor);
            var query = searchSolicitor.ToQuery();
            var result = await _mediator.Send(query, cancellation);
            _logger.LogInformation("SearchSolicitors returned {TotalCount} total results, {PageCount} on current page",
                result.TotalCount, result.Items.Count);

            return result;
        }

        [HttpGet("reports")]
        public async Task<ActionResult<ReportsDto>> GetReports(
            [FromQuery] int? topCount = 5,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GetReports endpoint called with topCount: {TopCount}", topCount);
            var query = new GetReportsQuery(topCount);
            var result = await _mediator.Send(query, cancellationToken);
            _logger.LogInformation("GetReports successfully returned reports data");

            return Ok(result);
        }
    }
}
