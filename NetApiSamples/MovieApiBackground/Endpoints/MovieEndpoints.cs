using CommonLibrary.Models.TMDB;
using CommonLibrary.Services;
using MovieApiBackground.BackgroundTasks;

namespace MovieApiBackground.Endpoints
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

            app.MapGet("/api/movie/requestqueuedmovies", (IBackgroundTaskQueue taskQueue) =>
            {
                return taskQueue.QueueBackgroundWorkItem();
            })
            .WithTags(MovieTag)
            .Produces<string>(StatusCodes.Status200OK);

            app.MapGet("/api/movie/getqueuedmovies", (IBackgroundTaskQueue taskQueue, string id) =>
            {
                return taskQueue.GetBackgroundTaskItem(id);
            })
            .WithTags(MovieTag)
            .Produces<BackgroundTaskItem>(StatusCodes.Status200OK);
        }
    }
}
