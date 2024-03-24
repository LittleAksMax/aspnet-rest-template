using Movies.Api.Auth.Extensions;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Movies;

public static class RateMovieEndpoint
{
    public static IEndpointRouteBuilder MapRateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Movies.Rate, async (
            IRatingService ratingService,
            [AsParameters] Guid id,
            [AsParameters] RateMovieRequest request,
            HttpContext context,
            CancellationToken token
        ) =>
        {
            var userId = context.GetUserId();
            var rating = request.Rating;

            var successfullyRated = await ratingService.RateMovieAsync(id, userId!.Value, rating, token);

            if (!successfullyRated)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        });
        return app;
    }
}