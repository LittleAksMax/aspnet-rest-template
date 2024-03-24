using Movies.Api.Endpoints.Ratings;

namespace Movies.Api.Extensions.Endpoints;

public static class RatingEndpointExtensions
{
    public static IEndpointRouteBuilder MapRatingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetRatingsForUser();
        return app;
    }
}