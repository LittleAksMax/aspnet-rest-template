namespace Movies.Contracts.Requests.Queries;

public class PagedQuery
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
    public int? Page { get; init; } = DefaultPage;
    public int? PageSize { get; init; } = DefaultPageSize;
}