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
                 id UUID PRIMARY KEY,
                 slug TEXT NOT NULL,
                 title TEXT NOT NULL,
                 yearofrelease INTEGER NOT NULL);
          """);
        
        // create unique index on slug since it will make searching more efficient
        await connection.ExecuteAsync("""
            CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS movies_slug_idx
            ON movies
            USING btree(slug);
          """);
        
        
        // create genres table
        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS genres (
                movieid UUID,
                name TEXT NOT NULL,
                PRIMARY KEY (movieid, name),
                CONSTRAINT fk_movieid
                    FOREIGN KEY (movieid)
                    REFERENCES movies (id)
                    ON DELETE CASCADE
            );
          """);

        // create ratings table
        await connection.ExecuteAsync("""
          CREATE TABLE IF NOT EXISTS ratings (
              userid UUID,
              movieid UUID,
              rating INTEGER NOT NULL,
              PRIMARY KEY (userid, movieid),
              CONSTRAINT fk_movieid
                   FOREIGN KEY (movieid)
                   REFERENCES movies (id)
                   ON DELETE CASCADE
          );
          """);
    }
}