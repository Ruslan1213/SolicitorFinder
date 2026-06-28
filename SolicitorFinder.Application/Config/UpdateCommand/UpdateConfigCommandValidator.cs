using FluentValidation;

namespace SolicitorFinder.Application.Config.UpdateCommand;

public sealed class UpdateConfigCommandValidator : AbstractValidator<UpdateConfigCommand>
{
    public UpdateConfigCommandValidator()
    {
        RuleFor(x => x.UpdateInterval)
            .InclusiveBetween(1, 86400)
            .WithMessage("Update interval must be between 1 second and 24 hours (86400 seconds)");

        RuleFor(x => x.MaxResults)
            .InclusiveBetween(1, 1000)
            .WithMessage("Max results must be between 1 and 1000");

        RuleFor(x => x.Locations)
            .NotNull()
            .WithMessage("Locations cannot be null")
            .Must(locations => locations == null || locations.Count > 0)
            .WithMessage("At least one location must be provided");

        RuleForEach(x => x.Locations)
            .ChildRules(location =>
            {
                location.RuleFor(l => l.Title)
                    .NotEmpty()
                    .WithMessage("Location title cannot be empty")
                    .MaximumLength(200)
                    .WithMessage("Location title cannot exceed 200 characters");

                location.RuleFor(l => l.Text)
                    .NotEmpty()
                    .WithMessage("Location text cannot be empty")
                    .MaximumLength(500)
                    .WithMessage("Location text cannot exceed 500 characters");
            });
    }
}
