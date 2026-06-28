using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Location.SearchLocation;

public sealed record SearchLocationsQuery(string SearchText) : IRequest<List<LocationDto>>;
