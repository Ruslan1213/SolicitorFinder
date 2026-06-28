using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Location.InitLocations;

public sealed record InitLocationsCommand(string[] InitLocationsNames) : IRequest<bool>;
