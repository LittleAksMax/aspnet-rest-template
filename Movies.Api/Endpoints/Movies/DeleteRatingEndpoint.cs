using Movies.Api.Auth.Extensions;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class DeleteRatingEndpoint
{
    public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Movies.DeleteRating, async (
            IRatingService ratingService,
            [AsParameters] Guid id,
            HttpContext context,
            CancellationToken token
        ) =>
        {
            var userId = context.GetUserId();
            var successfullyDeleted = await ratingService.DeleteRatingAsync(id, userId!.Value, token);

            if (!successfullyDeleted)
            {
                return Results.NotFound();
            }
            return Results.NoContent();
        });
        return app;
    }
}