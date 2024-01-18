using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonLibrary.Models.TMDB
{
    public class Movie
    {
        public string Title { get; set; }
        [JsonPropertyName("release_date")]
        public DateTime ReleaseDate { get; set; }
    }
}
