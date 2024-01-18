using CommonLibrary.Helpers;
using CommonLibrary.Models.TMDB;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonLibrary.Services
{
    public class TMDBCacheService : ITMDBCacheService
    {

        private readonly IMemoryCache _memoryCache;
        private readonly ITMDBService _tmdbService;

        private const string genreCacheKey = nameof(genreCacheKey);

        public TMDBCacheService(IMemoryCache memoryCache, ITMDBService tmdbService)
        {
            Guard.IsNotNull(memoryCache);
            Guard.IsNotNull(tmdbService);

            _memoryCache = memoryCache;
            _tmdbService = tmdbService;
        }

        public async Task<(GenreList list, bool isCached)> GetGenresAsync()
        {
            if(_memoryCache.TryGetValue<GenreList>(genreCacheKey, out var genres))
            {
                return (genres, true);
            }

            genres = await _tmdbService.GetGenresAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

            _memoryCache.Set(genreCacheKey, genres, cacheOptions);

            return (genres, false); 
        }
    }
}

