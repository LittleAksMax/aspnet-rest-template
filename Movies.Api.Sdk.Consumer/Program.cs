// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Movies.Contracts.Requests.Queries;
using Refit;

var services = new ServiceCollection();

services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (message, token) =>
            await s.GetRequiredService<AuthTokenProvider>().GetTokenAsync(message, token)
    })
    .ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5001"));

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

// var movie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
// {
//     Title = "Nick the Greek 2",
//     YearOfRelease = 2024,
//     Genres = new[] { "Action", "Comedy" }
// });

var movie = await moviesApi.GetMovieAsync("nick-the-greek-2023");
movie = await moviesApi.UpdateMovieAsync(movie.Id, new UpdateMovieRequest
{
    Title = "Nick the Greek",
    YearOfRelease = 2022,
    Genres = new[] { "Comedy" }
});
// var movies = await moviesApi.GetAllMoviesAsync(new GetAllMoviesQuery
// {
//     Title = null,
//     Year = null,
//     SortBy = null
// });

Console.WriteLine(JsonSerializer.Serialize(movie));