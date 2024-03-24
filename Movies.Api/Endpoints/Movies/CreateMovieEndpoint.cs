using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth.Constants;
using Movies.Api.Auth.Extensions;
using Movies.Api.Caching;
using Movies.Api.Mappers;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.Validation;

namespace Movies.Api.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Movies.Create, async (
            CreateMovieRequest request,
            IMovieService movieService,
            IOutputCacheStore cacheStore,
            HttpContext context,
            CancellationToken token
        ) =>
        {
            var userId = context.GetUserId();

            var movie = request.MapToMovie();
            await movieService.CreateAsync(movie, userId, token);

            // invalidate cache for this after successful creation
            await cacheStore.EvictByTagAsync(CachingConstants.MoviesCacheTag, token);
            
            // return appropriate response
            return TypedResults.CreatedAtRoute(movie.MapToMovieResponse(),
                GetMovieEndpoint.Name, new { idOrSlug = movie.Id });
        })
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName)
            .Produces<MovieResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest);
        return app;
    }
}