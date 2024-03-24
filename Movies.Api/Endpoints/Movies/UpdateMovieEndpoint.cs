using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth.Constants;
using Movies.Api.Auth.Extensions;
using Movies.Api.Caching;
using Movies.Api.Mappers;
using Movies.Api.Versioning;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.Validation;

namespace Movies.Api.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Movies.Update, async (
            IMovieService movieService,
            IOutputCacheStore cacheStore,
            [AsParameters] Guid id,
            [AsParameters] UpdateMovieRequest request,
            HttpContext context,
            CancellationToken token
        ) =>
        {
            var userId = context.GetUserId();

            var movie = request.MapToMovie(id);
            var updatedMovie = await movieService.UpdateAsync(movie, userId, token);
            if (updatedMovie is null)
            {
                return Results.NotFound();
            }

            // invalidate cache for this after successful creation
            await cacheStore.EvictByTagAsync(CachingConstants.MoviesCacheTag, token);

            return TypedResults.Ok(updatedMovie);
        })
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName)
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }
}