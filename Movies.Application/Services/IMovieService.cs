using Movies.Application.Models.Options;
using Movies.Contracts.Dtos;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(MovieDto movie, Guid? userId = default, CancellationToken token = default);
    Task<MovieDto?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);
    Task<MovieDto?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);
    Task<IEnumerable<MovieDto>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default);
    Task<MovieDto?> UpdateAsync(MovieDto movie, Guid? userId = default, CancellationToken token = default);
    Task<bool> DeleteById(Guid id, CancellationToken token = default);
    Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token);
}