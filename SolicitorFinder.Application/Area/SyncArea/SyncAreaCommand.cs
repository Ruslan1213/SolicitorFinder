using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Area.SyncArea;

public sealed record SyncAreaCommand() : IRequest<bool>;
