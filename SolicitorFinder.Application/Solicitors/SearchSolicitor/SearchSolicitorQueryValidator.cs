using FluentValidation;

namespace SolicitorFinder.Application.Solicitors.SearchSolicitor;

public sealed class SearchSolicitorQueryValidator : AbstractValidator<SearchSolicitorQuery>
{
    private static readonly string[] ValidSortFields = { "name", "ratingstars", "reviewcount" };

    public SearchSolicitorQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.MinRating)
            .InclusiveBetween(0.0, 5.0)
            .When(x => x.MinRating.HasValue)
            .WithMessage("Min rating must be between 0 and 5");

        RuleFor(x => x.MaxRating)
            .InclusiveBetween(0.0, 5.0)
            .When(x => x.MaxRating.HasValue)
            .WithMessage("Max rating must be between 0 and 5");

        RuleFor(x => x.MaxRating)
            .GreaterThanOrEqualTo(x => x.MinRating)
            .When(x => x.MinRating.HasValue && x.MaxRating.HasValue)
            .WithMessage("Max rating must be greater than or equal to min rating");

        RuleFor(x => x.MinReviewCount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinReviewCount.HasValue)
            .WithMessage("Min review count must be greater than or equal to 0");

        RuleFor(x => x.MaxReviewCount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxReviewCount.HasValue)
            .WithMessage("Max review count must be greater than or equal to 0");

        RuleFor(x => x.MaxReviewCount)
            .GreaterThanOrEqualTo(x => x.MinReviewCount)
            .When(x => x.MinReviewCount.HasValue && x.MaxReviewCount.HasValue)
            .WithMessage("Max review count must be greater than or equal to min review count");

        RuleFor(x => x.SortBy)
            .Must(sortBy => string.IsNullOrEmpty(sortBy) || ValidSortFields.Contains(sortBy.ToLower()))
            .WithMessage($"Sort by must be one of: {string.Join(", ", ValidSortFields)}");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage("Search term cannot exceed 200 characters");

        RuleFor(x => x.LocationId)
            .GreaterThan(0)
            .When(x => x.LocationId.HasValue)
            .WithMessage("Location ID must be greater than 0");

        RuleFor(x => x.AreaId)
            .GreaterThan(0)
            .When(x => x.AreaId.HasValue)
            .WithMessage("Area ID must be greater than 0");
    }
}
