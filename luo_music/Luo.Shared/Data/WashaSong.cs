using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Data
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public partial class WashaSong
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tracklist")]
        public bool Tracklist { get; set; }

        [JsonProperty("tracknumbers")]
        public bool Tracknumbers { get; set; }

        [JsonProperty("images")]
        public bool Images { get; set; }

        [JsonProperty("artists")]
        public bool Artists { get; set; }

        [JsonProperty("tracks")]
        public List<Track> Tracks { get; set; }
    }

    public class Image
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Thumb
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Track
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("thumb")]
        public Thumb Thumb { get; set; }
    }

    public class Meta
    {
        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("album")]
        public string Album { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("length_formatted")]
        public string Length_formatted { get; set; }
    }

    public partial class WashaSong
    {
        public static WashaSong FromJson(string json) => JsonConvert.DeserializeObject<WashaSong>(json);
    }

    public static class WashaSerialize
    {
        public static string ToJson(this List<WashaSong> self) => JsonConvert.SerializeObject(self, WashaConverter.Settings);
    }

    internal static class WashaConverter
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
