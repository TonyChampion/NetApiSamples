using CommonLibrary.Models.TMDB;
using CommonLibrary.Services;

namespace TokenBasedApi.Endpoints
{
    public static class MovieEndpoints
    {
        public static string MovieTag = "Movie";

        public static void MapMovieEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("getmovies", async (ITMDBService tmdbService, int? page) =>
            {
                return await tmdbService.GetMoviesExtendedAsync(page);
            })
            .WithTags(MovieTag)
            .Produces<MovieListPage>(StatusCodes.Status200OK);

            app.MapGet("genres", async (ITMDBService tmdbService) =>
            {
                return await tmdbService.GetGenresAsync();
            })
            .WithTags(MovieTag)
            .Produces<GenreList>(StatusCodes.Status200OK);
        }
    }

}
