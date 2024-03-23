using FluentValidation;
using Movies.Application.Models.Options;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] AllowedSortFields =
    {
        "title",
        "yearofrelease"
    };
    
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(x => x.SortField)
            .Must(x => x is null || AllowedSortFields.Contains(x, StringComparer.Ordinal))
            .WithMessage("You can only sort by 'title' or 'yearofrelease'.");
    }
}