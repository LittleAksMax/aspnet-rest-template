using FluentValidation;
using Movies.Application.Services;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Contracts.Requests.Queries.Queries.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;
    
    public MovieValidator(IMovieRepository movieRepository)
    {
        // for validating the slug
        _movieRepository = movieRepository;
        
        RuleFor(m => m.Id)
            .NotEmpty();
        RuleFor(m => m.Title)
            .NotEmpty();
        RuleFor(m => m.YearOfRelease)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(m => m.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system.");
        
        RuleFor(m => m.Genres)
            .NotEmpty();
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug, token: token);
        
        // if already in the database, then it must the same movie (update operation)
        // so we need to compare IDs
        if (existingMovie is not null)
        {
            return existingMovie.Id == movie.Id;
        }

        return existingMovie is null;
    }
}