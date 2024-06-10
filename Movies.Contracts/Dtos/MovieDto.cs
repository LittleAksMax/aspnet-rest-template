namespace Movies.Contracts.Dtos;

public class MovieDto
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Slug { get; init; }
    public float? Rating { get; set; }
    public int? UserRating { get; set; }
    public required int YearOfRelease { get; set; }
    public required List<string> Genres { get; init; } = new();
}