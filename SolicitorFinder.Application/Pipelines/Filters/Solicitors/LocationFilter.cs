namespace SolicitorFinder.Application.Pipelines.Filters.Solicitors;

public sealed class LocationFilter : Filter<Data.Models.Solicitor>
{
    public LocationFilter(int? locationId)
        : base(locationId.HasValue ? (s => s.SolicitorLocations.Any(x => x.LocationId == locationId)) : null)
    {
    }
}
