using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using CommonLibrary.Models.TMDB;

namespace CommonLibrary.Helpers
{
    public static class JsonSerializationHelper
    {
        public static JsonSerializerOptions CreateDefaultOptions(JsonSerializerContext context)
        {
            return new()
            {
                TypeInfoResolver = JsonSerializer.IsReflectionEnabledByDefault
                    ? new DefaultJsonTypeInfoResolver()
                    : context
            };
        }
    }

    [JsonSerializable(typeof(Genre[]))]
    public partial class GenreArrayJsonSerializerContext : JsonSerializerContext
    {
    }

    [JsonSerializable(typeof(GenreList))]
    [JsonSerializable(typeof(Genre[]))]
    public partial class GenreListJsonSerializerContext : JsonSerializerContext
    {

    }
}
