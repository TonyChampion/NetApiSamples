using CommonLibrary.Helpers;
using CommonLibrary.Models.TMDB;
using CommunityToolkit.Diagnostics;
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
    public class TMDBService : ITMDBService
    {
        public static string BaseUrl = "https://api.themoviedb.org/3/";
        public static readonly string GetGenres = "genre/movie/list?language=en";
        public static readonly string GetKeywordMovies = "keyword/{0}/movies?include_adult=true&language=en-US&page={1}";

        private readonly HttpClient _httpClient;
        private JsonSerializerOptions _jsonSerializerOptions = JsonSerializationHelper.CreateDefaultOptions(TMDBJsonSerializerContext.Default);

        public TMDBService(HttpClient httpClient)
        {
            Guard.IsNotNull(httpClient);
            _httpClient = httpClient;
        }

        public async Task<GenreList> GetGenresAsync()
        {
            return await _httpClient.GetFromJsonAsync<GenreList>(GetGenres);
        }

        public async Task<string> GetGenresAsJsonAsync()
        {
            return await _httpClient.GetStringAsync(GetGenres);
        }

        public async Task<MovieListPage> GetMoviesAsync(int? pageId)
        {
            string url = string.Format(GetKeywordMovies, GlobalConstants.MCUKeywordId, pageId ?? 1);
            return await _httpClient.GetFromJsonAsync<MovieListPage>(url);
       }

        public async Task<MovieExtendedListPage> GetMoviesExtendedAsync(int? pageId)
        {
            string url = string.Format(GetKeywordMovies, GlobalConstants.MCUKeywordId, pageId ?? 1);
            return await _httpClient.GetFromJsonAsync<MovieExtendedListPage>(url);
        }

        public async Task<string> GetMoviesAsJsonAsync(int? pageId)
        {
            string url = string.Format(GetKeywordMovies, GlobalConstants.MCUKeywordId, pageId ?? 1);
            return await _httpClient.GetStringAsync(url);
        }
    }

    [JsonSerializable(typeof(GenreList))]
    [JsonSerializable(typeof(Genre))]
    [JsonSerializable(typeof(IEnumerable<Genre>))]
    [JsonSerializable(typeof(Movie))]
    [JsonSerializable(typeof(Movie[]))]
    [JsonSerializable(typeof(IEnumerable<Movie>))]
    [JsonSerializable(typeof(Genre[]))]
    [JsonSerializable(typeof(MovieListPage))]
    internal partial class TMDBJsonSerializerContext : JsonSerializerContext
    {
    }
}

