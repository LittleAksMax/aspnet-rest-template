using Movies.Api.Auth.Extensions;
using Movies.Api.Mappers;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings;

public static class GetRatingsForUserEndpoint
{
    public static IEndpointRouteBuilder MapGetRatingsForUser(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Ratings.GetAllRatings, async (
            IRatingService ratingService,
            HttpContext context,
            CancellationToken token
        ) =>
        {
            var userId = context.GetUserId();
            var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
            return Results.Ok(ratings.MapToMovieRatingResponse());
        });
        return app;
    }
}