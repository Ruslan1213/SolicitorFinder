using SolicitorFinder.Application.Config.Dtos;
using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Config.UpdateCommand;

public sealed class UpdateConfigCommand : IRequest<ConfigDto>
{
    public int UpdateInterval { get; set; }

    public bool AutoUpdate { get; set; }

    public int MaxResults { get; set; }

    public List<LocationDto> Locations { get; set; } = new();
}
