namespace Movies.Contracts.Responses;

public class MovieRatingsResponse
{
    public required IEnumerable<MovieRatingResponse> Ratings { get; init; } = Enumerable.Empty<MovieRatingResponse>();
}