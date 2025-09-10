using CommonLibrary.Models.TMDB;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MovieApi.Endpoints
{
    public static class MovieEndpoints
    {
        public static string MovieTag = "movie";

        public static void MapMovieEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/movie/getmovies", async (ITMDBService tmdbService, int? page) =>
            {
                return await tmdbService.GetMoviesAsync(page);
            })
            .WithName("GetMovies")
            .WithTags(MovieTag)
            .Produces<MovieListPage>(StatusCodes.Status200OK);

            app.MapGet("/api/movie/genres", async (ITMDBService tmdbService) =>
            {
                return await tmdbService.GetGenresAsync();
            })
            .WithName("GetGenres")
            .WithTags(MovieTag)
            .Produces<GenreList>(StatusCodes.Status200OK);

            app.MapGet("/api/movie/headercachedgenres", async (ITMDBService tmdbService) =>
            {
                return await tmdbService.GetGenresAsync();
            })
            .WithName("GetHeaderCachedGenres")
            .Produces<GenreList>(StatusCodes.Status200OK)
            .WithTags(MovieTag)
            .CacheOutput(builder => builder.Expire(TimeSpan.FromSeconds(30)).Tag(MovieTag));

            app.MapGet("/api/movie/getMemorycachedgenres", async (ITMDBCacheService tmdbCacheService) =>
            {
                (GenreList list, bool isCached) = await tmdbCacheService.GetGenresAsync();
                return new GenreListCache()
                {
                    GenreList = list,
                    IsCached = isCached
                };
            })
            .WithName("GetMemoryCachedGenres")
            .WithTags(MovieTag)
            .Produces<GenreListCache>(StatusCodes.Status200OK);

            app.MapGet("/api/movie/getHybridCachedgenres", async (ITMDBCacheService tmdbCacheService) =>
            {
                return await tmdbCacheService.GetHybridCachedGenresAsync();
            })
            .WithName("GetHybridCachedGenres")
            .WithTags(MovieTag)
            .Produces<GenreListCache>(StatusCodes.Status200OK);

        }
    }
}
