using CommonLibrary.Helpers;
using CommonLibrary.Models.TMDB;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
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
using System.Xml.Linq;

namespace CommonLibrary.Services
{
    public class TMDBCacheService : ITMDBCacheService
    {

        private readonly IMemoryCache _memoryCache;
        private readonly ITMDBService _tmdbService;
        private readonly HybridCache _hybridCache;

        private const string genreCacheKey = nameof(genreCacheKey);

        public TMDBCacheService(IMemoryCache memoryCache, ITMDBService tmdbService, HybridCache hybridCache)
        {
            Guard.IsNotNull(memoryCache);
            Guard.IsNotNull(tmdbService);
            Guard.IsNotNull(hybridCache);

            _memoryCache = memoryCache;
            _tmdbService = tmdbService;
            _hybridCache = hybridCache;
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

        public async Task<GenreList> GetHybridCachedGenresAsync()
        {
            return await _hybridCache.GetOrCreateAsync(
                genreCacheKey,
                async cancel => await _tmdbService.GetGenresAsync()
            );
        }
    }
}

