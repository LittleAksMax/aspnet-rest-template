using Movies.Application.Models;
using Movies.Contracts.Dtos;

namespace Movies.Application.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default);
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
    Task<IEnumerable<MovieRatingDto>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}