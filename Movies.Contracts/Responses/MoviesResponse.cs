using Movies.Contracts.Responses.Pagination;

namespace Movies.Contracts.Responses;

public class MoviesResponse : PagedResponse
{
    public required IEnumerable<MovieResponse> Movies { get; init; } = Enumerable.Empty<MovieResponse>();
}