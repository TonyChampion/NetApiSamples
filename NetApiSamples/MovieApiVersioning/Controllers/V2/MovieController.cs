using Asp.Versioning;
using CommonLibrary.Models.TMDB;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace MovieApiVersioning.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(2.0)]
    public class MovieController : ControllerBase
    {

        private readonly ILogger<MovieController> _logger;
        private readonly ITMDBService _tmdbService;

        public MovieController(ILogger<MovieController> logger, ITMDBService tmdbService)
        {
            _logger = logger;
            _tmdbService = tmdbService;
        }

        [HttpGet("[action]")]
        public async Task<GenreList> GetGenres()
        {
            return await _tmdbService.GetGenresAsync();
        }


        [HttpGet("[action]")]
        public async Task<MovieExtendedListPage> GetMovies(int? page)
        {
            return await _tmdbService.GetMoviesExtendedAsync(page);
        }
    }
}
