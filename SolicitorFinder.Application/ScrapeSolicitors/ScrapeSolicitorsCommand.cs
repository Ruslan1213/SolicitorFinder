using SolicitorFinder.Application.ScrapeSolicitors.Dtos;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Solicitors.Commands.ScrapeSolicitors;

public sealed record ScrapeSolicitorsCommand() : IRequest<ScrapeResultDto>;
