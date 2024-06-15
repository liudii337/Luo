using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Data
{
    public class QueSong
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("album")]
        public string Album { get; set; }

        [JsonProperty("src")]
        public Uri Src { get; set; }

        [JsonProperty("pic")]
        public Uri Pic { get; set; }

        [JsonProperty("lrc")]
        public Uri Lrc { get; set; }

        [JsonProperty("journalNo")]
        public string JournalNo { get; set; }

        [JsonProperty("songNo")]
        public long SongNo { get; set; }

        [JsonProperty("haveCollect")]
        public bool HaveCollect { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }
    }

    public static class QueSerialize
    {
        public static string ToJson(this List<QueSong> self) => JsonConvert.SerializeObject(self);
    }
}
