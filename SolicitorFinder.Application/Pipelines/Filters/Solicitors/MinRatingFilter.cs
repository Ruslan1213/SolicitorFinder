namespace SolicitorFinder.Application.Pipelines.Filters.Solicitors;

public sealed class MinRatingFilter : Filter<Data.Models.Solicitor>
{
    public MinRatingFilter(double? minRating)
        : base(minRating.HasValue ?
            (s => s.RatingStars >= minRating.Value) : null)
    {
    }
}