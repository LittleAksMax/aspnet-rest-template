using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

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

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        return await _movieRepo.CreateAsync(movie, token);
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _movieRepo.GetByIdAsync(id, token);
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        return await _movieRepo.GetBySlugAsync(slug, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        return await _movieRepo.GetAllAsync(token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);

        // can't update if it doesn't exist
        if (!await _movieRepo.ExistsByIdAsync(movie.Id, token))
        {
            return null;
        }

        var updated = await _movieRepo.UpdateAsync(movie, token);
        if (!updated)
        {
            return null;
        }
        
        return movie;
    }

    public async Task<bool> DeleteById(Guid id, CancellationToken token = default)
    {
        return await _movieRepo.DeleteById(id, token);
    }
}