using Movies.Application.Models;
using Movies.Application.Models.Options;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie, Guid? userId = default, CancellationToken token = default);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(Movie movie, CancellationToken token = default);
    Task<bool> DeleteById(Guid id, CancellationToken token = default);
    Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token);
}