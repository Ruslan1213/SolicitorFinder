namespace SolicitorFinder.Application.Pipelines.Filters.Solicitors;

public sealed class MinReviewCountFilter : Filter<Data.Models.Solicitor>
{
    public MinReviewCountFilter(int? minReviewCount)
        : base(minReviewCount.HasValue ?
            (s => s.ReviewCount >= minReviewCount.Value) : null)
    {
    }
}
