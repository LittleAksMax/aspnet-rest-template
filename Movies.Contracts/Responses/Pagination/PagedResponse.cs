namespace Movies.Contracts.Responses.Pagination;

public class PagedResponse
{
    public required int PageSize { get; init; }
    public required int Page { get; init; }
    public required int Total { get; init; }
    public required string? Prev { get; init; }
    public required string? Next { get; init; }
}