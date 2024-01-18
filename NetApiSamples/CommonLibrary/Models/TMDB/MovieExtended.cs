using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonLibrary.Models.TMDB
{
    public class MovieExtended : Movie
    {
        public bool Adult { get; set; }
        public string Overview { get; set; }
    }
}
