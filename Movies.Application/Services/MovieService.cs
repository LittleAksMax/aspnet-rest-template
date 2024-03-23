using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Models.Options;
using Movies.Application.Repositories;

namespace Movies.Contracts.Requests.Queries.Queries.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepo;
    private readonly IRatingRepository _ratingRepo;
    private readonly IValidator<Movie> _movieValidator;
    private readonly IValidator<GetAllMoviesOptions> _optionsValidator;

    public MovieService(IMovieRepository movieRepo, IRatingRepository ratingRepo, IValidator<Movie> movieValidator, IValidator<GetAllMoviesOptions> optionsValidator)
    {
        _movieRepo = movieRepo;
        _movieValidator = movieValidator;
        _optionsValidator = optionsValidator;
        _ratingRepo = ratingRepo;
    }

    public async Task<bool> CreateAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        return await _movieRepo.CreateAsync(movie, userId, token);
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        return await _movieRepo.GetByIdAsync(id, userId, token);
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        return await _movieRepo.GetBySlugAsync(slug, userId, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);
        return await _movieRepo.GetAllAsync(options, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
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

        // we can only add the user-specific data if we were passed
        // the user ID
        if (!userId.HasValue)
        {
            var rating = await _ratingRepo.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
        }
        else
        {
            var ratings = await _ratingRepo.GetRatingAsync(movie.Id, userId.Value, token);
            movie.Rating = ratings.Rating;
            movie.UserRating = ratings.UserRating;
        }
        
        return movie;
    }

    public async Task<bool> DeleteById(Guid id, CancellationToken token = default)
    {
        return await _movieRepo.DeleteById(id, token);
    }

    public async Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token)
    {
        return await _movieRepo.GetCountAsync(title, yearOfRelease, token);
    }
}