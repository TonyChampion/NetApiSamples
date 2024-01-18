using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonLibrary.Models.TMDB
{
    public class MovieListPage
    {
        public int Id { get; set; }
        public int Page { get; set; }
        public IEnumerable<Movie> Results { get; set; } = Enumerable.Empty<Movie>();
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }   
    }
}
