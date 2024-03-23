namespace Movies.Contracts.Requests.Queries;

public class PagedQuery
{
    public required int Page { get; init; } = 1;
    public required int PageSize { get; init; } = 10;
}