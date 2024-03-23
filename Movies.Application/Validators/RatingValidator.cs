using FluentValidation;

namespace Movies.Contracts.Requests.Queries.Queries.Application.Validators;

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