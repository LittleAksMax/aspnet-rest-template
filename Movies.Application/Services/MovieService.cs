using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Validators;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepo;
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(IMovieRepository movieRepo, IValidator<Movie> movieValidator)
    {
        _movieRepo = movieRepo;
        _movieValidator = movieValidator;
    }

    public async Task<bool> CreateAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        return await _movieRepo.CreateAsync(movie);
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        return await _movieRepo.GetByIdAsync(id);
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        return await _movieRepo.GetBySlugAsync(slug);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _movieRepo.GetAllAsync();
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);

        // can't update if it doesn't exist
        if (!await _movieRepo.ExistsByIdAsync(movie.Id))
        {
            return null;
        }

        var updated = await _movieRepo.UpdateAsync(movie);
        if (!updated)
        {
            return null;
        }
        
        return movie;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        return await _movieRepo.DeleteById(id);
    }
}