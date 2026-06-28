namespace SolicitorFinder.Application.Pipelines.Filters.Solicitors;

public sealed class MaxReviewCountFilter : Filter<Data.Models.Solicitor>
{
    public MaxReviewCountFilter(int? maxReviewCount)
        : base(maxReviewCount.HasValue ?
            (s => s.ReviewCount <= maxReviewCount.Value) : null)
    {
    }
}
