using FluentValidation;

namespace Movies.Application.Validators;

public class RatingValidator : AbstractValidator<float>
{
    public RatingValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .LessThanOrEqualTo(5)
            .GreaterThanOrEqualTo(1);
    }
}