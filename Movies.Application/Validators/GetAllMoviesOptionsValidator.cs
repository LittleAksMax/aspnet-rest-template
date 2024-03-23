using FluentValidation;
using Movies.Application.Models.Options;

namespace Movies.Contracts.Requests.Queries.Queries.Application.Validators;

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
        RuleFor(x => x.Page)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize)
            .NotEmpty()
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(25);
    }
}