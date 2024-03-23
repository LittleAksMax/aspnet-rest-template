namespace Movies.Application.Models.Options;

public class GetAllMoviesOptions
{
    public string? Title { get; set; }
    public int? YearOfRelease { get; set; }
    public Guid? UserId { get; set; }
    public string? SortField { get; set; }
    public SortOrder? SortOrder { get; set; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}