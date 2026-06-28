using FluentValidation;

namespace SolicitorFinder.Application.Report;

public sealed class GetReportsQueryValidator : AbstractValidator<GetReportsQuery>
{
    public GetReportsQueryValidator()
    {
        RuleFor(x => x.TopCount)
            .InclusiveBetween(1, 100)
            .When(x => x.TopCount.HasValue)
            .WithMessage("Top count must be between 1 and 100");
    }
}
