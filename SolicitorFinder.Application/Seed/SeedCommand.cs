using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Seed;

public sealed record SeedCommand() : IRequest<bool>;
