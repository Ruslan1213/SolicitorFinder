namespace SolicitorFinder.Application.Pipelines.Filters.Solicitors;

public sealed class MaxRatingFilter : Filter<Data.Models.Solicitor>
{
    public MaxRatingFilter(double? maxRating)
        : base(maxRating.HasValue ?
            (s => s.RatingStars <= maxRating.Value) : null)
    {
    }
}