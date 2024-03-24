using Movies.Api.Auth.Extensions;
using Movies.Api.Mappers;
using Movies.Api.Versioning;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies;

public static class GetMovieEndpoint
{
    private const string Name = "GetMovie";
    public const string NameV1 = $"{Name}V1";

    public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Movies.Get, async (
                string idOrSlug,
                IMovieService movieService,
                HttpContext context,
                CancellationToken token) =>
            {
                var userId = context.GetUserId();

                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await movieService.GetByIdAsync(id, userId, token)
                    : await movieService.GetBySlugAsync(idOrSlug, userId, token);

                if (movie is null)
                {
                    return Results.NotFound();
                }

                return TypedResults.Ok(movie.MapToMovieResponse());
            })
            .WithName(NameV1) // so it can be identified
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }
}