using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Location.GetLocations;

public sealed record GetLocationsQuery() : IRequest<List<LocationIdDto>>;
