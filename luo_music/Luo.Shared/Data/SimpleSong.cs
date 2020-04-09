using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Luo.Shared.Data
{
    public partial class SimpleSong
    {
        [JsonProperty("src")]
        public Uri Src { get; set; }

        [JsonProperty("song_id")]
        public string SongId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }
    }

    public partial class SimpleSong
    {
        public static List<SimpleSong> FromJson(string json) => JsonConvert.DeserializeObject<List<SimpleSong>>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<SimpleSong> self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
