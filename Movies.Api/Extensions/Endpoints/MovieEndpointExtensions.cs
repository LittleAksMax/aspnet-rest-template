using Movies.Api.Endpoints.Movies;

namespace Movies.Api.Extensions.Endpoints;

public static class MovieEndpointExtensions
{
    public static IEndpointRouteBuilder MapMovieEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetMovie();
        app.MapGetAllMovies();
        app.MapUpdateMovie();
        app.MapCreateMovie();
        app.MapDeleteMovie();

        app.MapRateMovie();
        app.MapDeleteRating();
        return app;
    }
}