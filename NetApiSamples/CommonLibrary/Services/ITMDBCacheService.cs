using CommonLibrary.Models.TMDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Services
{
    public interface ITMDBCacheService
    {
        Task<(GenreList list, bool isCached)> GetGenresAsync();
    }
}
