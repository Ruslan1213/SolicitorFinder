using SolicitorFinder.Application.Config.Dtos;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Config.GetConfig;

public record GetConfigQuery : IRequest<ConfigDto>
{
}
