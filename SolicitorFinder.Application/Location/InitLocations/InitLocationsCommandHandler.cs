using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Mediator.Interfaces;
using SolicitorFinder.Services.Interfaces;

namespace SolicitorFinder.Application.Location.InitLocations;
public sealed class InitLocationsCommandHandler : IRequestHandler<InitLocationsCommand, bool>
{
    private readonly IBaseRepository<Data.Models.Location> _locationRepository;
    private readonly ILocationService _locationService;
    private readonly IUnitOfWork _unitOfWork;

    public InitLocationsCommandHandler(
        ILocationService locationService,
        IBaseRepository<Data.Models.Location> locationRepository,
        IUnitOfWork unitOfWork)
    {
        _locationService = locationService;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(InitLocationsCommand request, CancellationToken cancellationToken)
    {
        foreach (var location in request.InitLocationsNames)
        {
            var locations = await _locationService.SearchLocationsAsync(location);
            var dbLocations = locations.Select(x => new Data.Models.Location
            {
                Title = x.Title,
                Text = x.Text,
            });

            await _locationRepository.AddRangeAsync(dbLocations, cancellationToken);
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;

        return result;
    }
}
