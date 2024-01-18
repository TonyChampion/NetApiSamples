using CommonLibrary.Models.TMDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Services
{
    public interface ITMDBService
    {
        Task<MovieListPage> GetMoviesAsync(int? pageId);
        Task<MovieExtendedListPage> GetMoviesExtendedAsync(int? pageId);

        Task<GenreList> GetGenresAsync();
        Task<string> GetMoviesAsJsonAsync(int? pageId);
        Task<string> GetGenresAsJsonAsync();
    }
}
