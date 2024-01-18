using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Models.TMDB
{
    public class GenreListCache
    {
        public GenreList GenreList { get; set; }
        public bool IsCached { get; set; }
    }
}
