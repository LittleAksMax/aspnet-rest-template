namespace Movies.Contracts.Requests.Queries;

public class GetAllMoviesQuery
{
    public string? Title { get; init; }
    public int? Year { get; init; }
    public string? SortBy { get; init; }
}