using CommonLibrary.Models.TMDB;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TokenBasedApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult<GenreList>> GetGenres()
        {
            return await _tmdbService.GetGenresAsync();
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<MovieExtendedListPage>> GetMovies(int? page)
        {
            return await _tmdbService.GetMoviesExtendedAsync(page);
        }
    }
}
