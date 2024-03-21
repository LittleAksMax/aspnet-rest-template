using Dapper;

namespace Movies.Application.Database;

public class DbInitialiser
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitialiser(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitialiseAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        // create movies table if it doesn't already exist
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS movies (
                                             id UUID primary key,
                                             slug TEXT NOT NULL,
                                             title TEXT NOT NULL,
                                             autorelease INTEGER NOT NULL);
                                      """);
        
        // create unique index on slug since it will make searching more efficient
        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS movies_slug_idx
                                      ON movies
                                      using btree(slug);
                                      """);
        
        
    }
}