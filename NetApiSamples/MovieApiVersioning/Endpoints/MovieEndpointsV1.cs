using Asp.Versioning;
using Asp.Versioning.Builder;
using CommonLibrary.Models.TMDB;
using CommonLibrary.Services;

namespace MovieApiVersioning.Endpoints
{
    public static class MovieEndpointsV1
    {
        public static string MovieTag = "Movie";

        public static void MapMovieV1Endpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("movie/getmovies", async (ITMDBService tmdbService, int? page) =>
            {
                return await tmdbService.GetMoviesAsync(page);
            })
            .MapToApiVersion(new ApiVersion(1))
            .WithTags(MovieTag)
            .Produces<MovieListPage>(StatusCodes.Status200OK);

            app.MapGet("movie/genres", async (ITMDBService tmdbService) =>
            {
                return await tmdbService.GetGenresAsync();
            })
            .MapToApiVersion(new ApiVersion(1))
            .WithTags(MovieTag)
            .Produces<GenreList>(StatusCodes.Status200OK);
        }
    }
}
