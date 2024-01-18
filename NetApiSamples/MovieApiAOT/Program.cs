using CommonLibrary.Services;
using CommonLibrary;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using CommonLibrary.Models.TMDB;
using Microsoft.Extensions.Options;
using System.Text.Json;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Add services to the container.
builder.Services.AddHttpClient<ITMDBService, TMDBService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(TMDBService.BaseUrl);
    var test = configuration[GlobalConstants.TMDBApiKey];
    client.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", configuration[GlobalConstants.TMDBApiKey]);
});

var app = builder.Build();

var tmdbClient = app.Services.GetRequiredService<ITMDBService>();

var movieApi = app.MapGroup("/Movie");

movieApi.MapGet("/GetGenres", async () =>
{
   return Results.Ok(await tmdbClient.GetGenresAsJsonAsync());
});

movieApi.MapGet("/GetMovies", async (int? page) =>
{
   return Results.Ok(await tmdbClient.GetMoviesAsJsonAsync(page));
});

app.Run();

[JsonSerializable(typeof(GenreList))]
[JsonSerializable(typeof(Genre))]
[JsonSerializable(typeof(Movie))]
[JsonSerializable(typeof(MovieListPage))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}