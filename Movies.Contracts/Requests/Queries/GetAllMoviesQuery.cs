namespace Movies.Contracts.Requests.Queries;

public class GetAllMoviesQuery
{
    public required string? Title { get; init; }
    public required int? Year { get; init; }
    public required string? SortBy { get; init; }
}