using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RatingRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(
            new CommandDefinition("""
                                  INSERT INTO ratings (userid, movieid, rating)
                                  VALUES (@userId, @movieId, @rating)
                                  ON CONFLICT (userid, movieid) DO UPDATE SET rating = @rating;
                                  """, new { movieId, userId, rating }, cancellationToken: token));
        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<float?>(
            new CommandDefinition("""
              SELECT round(avg(rating), 1)
              FROM ratings
              WHERE movieid=@movieId;
              """, new { movieId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(
            new CommandDefinition("""
              SELECT round(avg(rating), 1), (SELECT rating
                                             FROM ratings
                                             WHERE userid=@userId AND movieid=@movieId
                                             LIMIT 1)
              FROM ratings
              WHERE movieid=@movieId;
              """, new { movieId, userId }, cancellationToken: token));

    }
}