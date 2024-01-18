using CommonLibrary.Models.TMDB;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace MovieApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {

        private readonly ILogger<MovieController> _logger;
        private readonly ITMDBService _tmdbService;
        private readonly ITMDBCacheService _tmdbCacheService;

        public MovieController(ILogger<MovieController> logger, ITMDBService tmdbService, ITMDBCacheService tmdbCacheService)
        {
            _logger = logger;
            _tmdbService = tmdbService;
            _tmdbCacheService = tmdbCacheService;
        }

        [HttpGet("[action]")]
        public async Task<GenreList> GetGenres()
        {
            return await _tmdbService.GetGenresAsync();
        }

        [HttpGet("[action]")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<GenreList> GetHeaderCachedGenres()
        {
            return await _tmdbService.GetGenresAsync();
        }

        [HttpGet("[action]")]
        public async Task<GenreListCache> GetMemoryCachedGenres()
        {
            (GenreList list, bool isCached) = await _tmdbCacheService.GetGenresAsync();
            return new GenreListCache()
            {
                GenreList = list,
                IsCached = isCached
            };
        }

        [HttpGet("[action]")]
        public async Task<MovieListPage> GetMovies(int? page)
        {
            return await _tmdbService.GetMoviesAsync(page);
        }
    }
}
