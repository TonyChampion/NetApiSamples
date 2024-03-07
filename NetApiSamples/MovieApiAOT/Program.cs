using CommonLibrary.Services;
using CommonLibrary;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using CommonLibrary.Models.TMDB;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using CommonLibrary.Helpers;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = new GenreListJsonSerializerContext();

    //options.SerializerOptions.TypeInfoResolverChain.Insert(0, GenreArrayJsonSerializerContext.Default);
   // options.SerializerOptions.TypeInfoResolverChain.Insert(0, GenreListJsonSerializerContext.Default);
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
    var genres = await tmdbClient.GetGenresAsync();

   return Results.Ok(genres);
});

movieApi.MapGet("/GetMovies", async (int? page) =>
{
   return Results.Ok(await tmdbClient.GetMoviesAsJsonAsync(page));
});

app.Run();

