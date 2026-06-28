namespace SolicitorFinder.Application.Pipelines.Filters.Solicitors;

public sealed class AreaIdFilter : Filter<Data.Models.Solicitor>
{
    public AreaIdFilter(int? areaId)
        : base(areaId.HasValue ? (s => s.SolicitorAreas != null && s.SolicitorAreas.Any(x => x.AreaId == areaId)) : null)
    {
    }
}
