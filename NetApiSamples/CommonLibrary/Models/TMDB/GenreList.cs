﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonLibrary.Models.TMDB
{
    public class GenreList
    {
        [JsonPropertyName("genres")]
        public IEnumerable<Genre> Genres { get; set; }
    }
}
