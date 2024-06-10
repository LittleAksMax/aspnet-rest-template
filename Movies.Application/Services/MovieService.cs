using FluentValidation;
using Movies.Application.Mappers;
using Movies.Application.Models;
using Movies.Application.Models.Options;
using Movies.Application.Repositories;
using Movies.Contracts.Dtos;

namespace Movies.Application.Services;

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

    public async Task<bool> CreateAsync(MovieDto movie, Guid? userId = default, CancellationToken token = default)
    {
        var m = movie.ToDom();
        await _movieValidator.ValidateAndThrowAsync(m, token);
        return await _movieRepo.CreateAsync(m, userId, token);
    }

    public async Task<MovieDto?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        var m = await _movieRepo.GetByIdAsync(id, userId, token);
        return m?.ToDto();
    }

    public async Task<MovieDto?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        var m = await _movieRepo.GetBySlugAsync(slug, userId, token);
        return m?.ToDto();
    }

    public async Task<IEnumerable<MovieDto>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);
        var ms = await _movieRepo.GetAllAsync(options, token);
        return ms.Select(m => m.ToDto());
    }

    public async Task<MovieDto?> UpdateAsync(MovieDto movie, Guid? userId = default, CancellationToken token = default)
    {
        var m = movie.ToDom();
        await _movieValidator.ValidateAndThrowAsync(m, token);

        // can't update if it doesn't exist
        if (!await _movieRepo.ExistsByIdAsync(m.Id, token))
        {
            return null;
        }

        var updated = await _movieRepo.UpdateAsync(m, token);
        if (!updated)
        {
            return null;
        }

        // we can only add the user-specific data if we were passed
        // the user ID
        if (!userId.HasValue)
        {
            var rating = await _ratingRepo.GetRatingAsync(m.Id, token);
            m.Rating = rating;
        }
        else
        {
            var ratings = await _ratingRepo.GetRatingAsync(m.Id, userId.Value, token);
            m.Rating = ratings.Rating;
            m.UserRating = ratings.UserRating;
        }
        
        return m.ToDto();
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