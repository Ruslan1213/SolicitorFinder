using SolicitorFinder.Services.Models;

namespace SolicitorFinder.Services.Interfaces;
public interface ILocationService
{
    Task<List<LocationResponse>> SearchLocationsAsync(string query, CancellationToken cancellationToken = default);
}