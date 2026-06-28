using SolicitorFinder.Application.Area.Dto;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Area;

public sealed record GetAreasQuery() : IRequest<List<AreaInfo>>;
