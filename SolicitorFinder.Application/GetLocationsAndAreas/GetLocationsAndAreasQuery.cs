using SolicitorFinder.Application.GetLocationsAndAreas.Dtos;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.GetLocationsAndAreas;

public sealed record GetLocationsAndAreasQuery() : IRequest<LocationsAndAreasDto>;
