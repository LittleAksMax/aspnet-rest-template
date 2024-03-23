using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Contracts.Requests.Queries.Queries.Application.Services;

public class RatingService : IRatingService
{
    private readonly IValidator<float> _ratingValidator;
    private readonly IRatingRepository _ratingRepo;
    private readonly IMovieRepository _movieRepo;

    public RatingService(IRatingRepository ratingRepo, IMovieRepository movieRepo, IValidator<float> ratingValidator)
    {
        _ratingRepo = ratingRepo;
        _movieRepo = movieRepo;
        _ratingValidator = ratingValidator;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
    {
        await _ratingValidator.ValidateAndThrowAsync(rating, token);

        var movieExists = await _movieRepo.ExistsByIdAsync(movieId, token);
        
        // can't rate non-existent movie
        if (!movieExists)
        {
            return false;
        }

        return await _ratingRepo.RateMovieAsync(movieId, userId, rating, token);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        return await _ratingRepo.DeleteRatingAsync(movieId, userId, token);
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        return await _ratingRepo.GetRatingsForUserAsync(userId, token);
    }
}