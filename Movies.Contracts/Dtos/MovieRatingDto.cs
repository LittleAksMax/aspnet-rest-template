namespace Movies.Contracts.Dtos;

public class MovieRatingDto
{
    public required Guid MovieId { get; init; }
    public required string Slug { get; init; }
    public required int Rating { get; init; }
}