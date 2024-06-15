using HtmlAgilityPack;
using Luo.Shared.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Luo.Shared.Data
{
    public class LuoVol : ModelBase
    {
        private string volnum;
        public string VolNum
        {
            get
            {
                return volnum;
            }
            set
            {
                if (volnum != value)
                {
                    volnum = value;
                    RaisePropertyChanged(() => VolNum);
                }
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private string volurl;
        public string VolUrl
        {
            get
            {
                return volurl;
            }
            set
            {
                if (volurl != value)
                {
                    volurl = value;
                    RaisePropertyChanged(() => VolUrl);
                }
            }
        }

        private ObservableCollection<string> tags;
        [IgnoreDataMember]
        public ObservableCollection<string> Tags
        {
            get
            {
                return tags;
            }
            set
            {
                if (tags != value)
                {
                    tags = value;
                    RaisePropertyChanged(() => Tags);
                }
            }
        }

        private string cover;
        public string Cover
        {
            get
            {
                return cover;
            }
            set
            {
                if (cover != value)
                {
                    cover = value;
                    RaisePropertyChanged(() => Cover);
                }
            }
        }

        private string description;
        [IgnoreDataMember]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (description != value)
                {
                    description = value;
                    RaisePropertyChanged(() => Description);
                }
            }
        }

        private string date;
        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                if (date != value)
                {
                    date = value;
                    RaisePropertyChanged(() => Date);
                }
            }
        }

        private string favdcount;
        public string FavdCount
        {
            get
            {
                return favdcount;
            }
            set
            {
                if (favdcount != value)
                {
                    favdcount = value;
                    RaisePropertyChanged(() => FavdCount);
                }
            }
        }

        private string commentcount;
        public string CommentCount
        {
            get
            {
                return commentcount;
            }
            set
            {
                if (commentcount != value)
                {
                    commentcount = value;
                    RaisePropertyChanged(() => CommentCount);
                }
            }

        }

        private bool isVolSongOnline;
        [IgnoreDataMember]
        public bool IsVolSongOnline
        {
            get
            {
                return isVolSongOnline;
            }
            set
            {
                if (isVolSongOnline != value)
                {
                    isVolSongOnline = value;
                    RaisePropertyChanged(() => IsVolSongOnline);
                }
            }
        }

        [IgnoreDataMember]
        public bool IsDetailGet = false;

        private ObservableCollection<LuoVolSong> volSongs;
        [IgnoreDataMember]
        public ObservableCollection<LuoVolSong> VolSongs
        {
            get
            {
                return volSongs;
            }
            set
            {
                if (volSongs != value)
                {
                    volSongs = value;
                    RaisePropertyChanged(() => VolSongs);
                }
            }
        }

        public LuoVol()
        {
        }

        public LuoVol(string _cover,string _volnum, string _volurl, string _title, string _commentcount, string _favdcount)
        {
            Cover = _cover;
            VolNum = _volnum;
            VolUrl = _volurl;
            Title = _title;
            CommentCount = _commentcount;
            FavdCount = _favdcount;
        }

        public LuoVol(string _cover, string _volnum, string _volurl, string _title)
        {
            Cover = _cover;
            VolNum = _volnum;
            VolUrl = _volurl;
            Title = _title;
            CommentCount = "1314";
            FavdCount = "999";
        }

        public void GetDetailVol(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            //查找节点
            //var vol_number = doc.DocumentNode.SelectSingleNode("//span[@class='vol-number rounded']").InnerText;
            //var vol_title = doc.DocumentNode.SelectSingleNode("//span[@class='vol-title']").InnerText;
            //var vol_img = doc.DocumentNode.SelectSingleNode("//*[@id='volCoverWrapper']/img").GetAttributeValue("src", "");

            var a = doc.DocumentNode.SelectSingleNode("//div[@class='vol-desc']").SelectNodes("./p");
            var b = "";
            if(a != null)
            {
                foreach (var i in a)
                {
                    if (i.InnerText == "br")
                    {
                        b = b + "\n";
                    }
                    else
                    {
                        b = b + i.InnerText + "\n";
                    }
                }
            }
            else
            {
                b = doc.DocumentNode.SelectSingleNode("//div[@class='vol-desc']").InnerHtml.HtmlParse();
            }

            Description=b.DescripitionParse();

            Date = doc.DocumentNode.SelectSingleNode("//span[@class='vol-date']").InnerText;

            Tags = new ObservableCollection<String>();
            var vol_tags = doc.DocumentNode.SelectNodes("//a[@class='vol-tag-item']");
            if(vol_tags != null)
            {
                foreach (var i in vol_tags)
                {
                    //Tags.Add(i.InnerText.Replace("#", ""));
                    Tags.Add(i.InnerText);
                }
            }

            VolSongs = new ObservableCollection<LuoVolSong>();
            var list = doc.DocumentNode.SelectNodes("//li[@class='track-item rounded']");
            foreach (var i in list)
            {
                var string1 = i.SelectSingleNode("./div[1]/a[1]").InnerText;
                var _index = string1.Substring(0, 2);
                var _name = string1.Remove(0, 4);
                var _artist=i.SelectSingleNode("./div[1]/span[2]").InnerText;
                var _album = i.SelectSingleNode("./div[2]/div[2]/div[1]/p[3]").InnerText.Remove(0, 7);
                var imagesrc = i.SelectSingleNode("./div[1]/a[3]").GetAttributeValue("data-img", "");
                VolSongs.Add(new LuoVolSong(VolNum,_index, _name, _artist,_album, imagesrc));
            }
            IsDetailGet = true;
        }

        public void GetDetailVol_w(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            if(Title.IsNullorEmpty())
            {
                var vol_title = doc.DocumentNode.SelectSingleNode("//div[@class='title']").InnerText;
                Title=vol_title.Substring(vol_title.IndexOf(' ') + 1);
            }

            if (Cover.IsNullorEmpty())
            {
                Cover = doc.DocumentNode.SelectSingleNode("//img[@class='img-responsive']").GetAttributeValue("src", "");
            }

            //查找节点
            //var vol_number = doc.DocumentNode.SelectSingleNode("//span[@class='vol-number rounded']").InnerText;
            //var vol_title = doc.DocumentNode.SelectSingleNode("//span[@class='vol-title']").InnerText;
            //var vol_img = doc.DocumentNode.SelectSingleNode("//*[@id='volCoverWrapper']/img").GetAttributeValue("src", "");

            var a = doc.DocumentNode.SelectSingleNode("//div[@class='vol-desc']").SelectNodes("./p");
            var b = "";
            if (a != null)
            {
                if (a.Count == 1)
                {
                    b = a[0].InnerHtml.HtmlParse_w();
                }
                else
                {
                    foreach (var i in a)
                    {
                        if (i.InnerText == "br")
                        {
                            b = b + "\n";
                        }
                        else
                        {
                            b = b + i.InnerText + "\n";
                        }
                        if (i.InnerText.Contains("Cover"))
                        { break; }
                    }
                }
            }
            else
            {
                b = doc.DocumentNode.SelectSingleNode("//div[@class='vol-desc']").InnerHtml.HtmlParse_w();
            }

            Description = b.DescripitionParse();


            //Date = doc.DocumentNode.SelectSingleNode("//span[@class='vol-date']").InnerText;

            Tags = new ObservableCollection<String>();
            var vol_tags = doc.DocumentNode.SelectNodes("//a[@target='_blank']");

            if (vol_tags != null)
            {
                foreach (var i in vol_tags)
                {
                    Tags.Add("#"+ i.InnerText.Replace("&nbsp", ""));
                }
            }

            var script = doc.DocumentNode.Descendants("script").ToArray().First(p => p.OuterHtml.Contains("var player")).OuterHtml;

            IsVolSongOnline = script.Contains("cloud");

            //if (script != null)
            //{
            //    var SourceNum = Regex.Replace(script, @"[^\d.\d]", "");
            //}

            //int b1 = script.IndexOf("[");//找a的位置
            //int b2 = script.LastIndexOf("]");//找b的位置
            //var jsonString = (script.Substring(b1)).Substring(0, b2 - b1 + 1);

            //var list = SimpleSong.FromJson(jsonString);

            //foreach (var i in list)
            //{
            //    var _index = i.Name.Substring(0, 2);
            //    var _name = i.Name.Substring(4);
            //    var _artist = i.Author;
            //    var _songsrc = i.Src.OriginalString;

            //    if(!VolSongs.Any(p=>p.Index==_index))
            //    {
            //        VolSongs.Add(new LuoVolSong(VolNum, _index, _name, _artist, _songsrc));
            //    }
            //}

            //check if there is repeat volSongs

            IsDetailGet = true;
        }

        public void GetDetailVol_q(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            if (Title.IsNullorEmpty())
            {
                Title = doc.DocumentNode.SelectSingleNode("//div[@class='w-autp text-[24px] leading-[33.6px] my-[9px] text-base font-bold text-overflow']").InnerText; 
            }

            if (Cover.IsNullorEmpty())
            {
                Cover = doc.DocumentNode.SelectSingleNode("//img[@class='absolute top-50% left-0 transform-translate-y-[-50%] w-full h-auto object-cover object-c false']").GetAttributeValue("src", "");
            }

            var a = doc.DocumentNode.SelectSingleNode("//article[@class='mt-[24px] text-[15px] leading-[21px] text-base']").SelectNodes("./p");
            var b = "";
            if (a != null)
            {
                foreach (var i in a)
                {
                    if (i.InnerText == "br")
                    {
                        b = b + "\n";
                    }
                    else
                    {
                        b = b + i.InnerText + "\n";
                    }
                    if (i.InnerText.Contains("Cover"))
                    { break; }
                }
            }
            else
            { b = doc.DocumentNode.SelectSingleNode("//article[@class='mt-[24px] text-[15px] leading-[21px] text-base']").InnerText; }

            Description = b.Replace("&nbsp;", "");

            //Date = doc.DocumentNode.SelectSingleNode("//span[@class='vol-date']").InnerText;

            Tags = new ObservableCollection<String>();
            var vol_tags = doc.DocumentNode.SelectNodes("//span[@class='block w-fit py-[3px] px-[10px] rounded-[15px] bg-[rgba(0,0,0,0.05)] text-[rgba(0,0,0,0.7)] text-[12px] leading-[12px]']");

            if (vol_tags != null)
            {
                foreach (var i in vol_tags)
                {
                    Tags.Add("#" + i.InnerText);
                }
            }

            var keystring1 = "songList\":[";
            var keystring2 = "}]}]]}],";
            int startIndex = html.IndexOf("songList", 1) + keystring1.Length;
            int endIndex = html.IndexOf(keystring2, 1) + 2;
            var jsonstring = html.Substring(startIndex, endIndex - startIndex).Replace("\\", "");

            if (VolSongs == null)
            { VolSongs = new ObservableCollection<LuoVolSong>(); }
            else
            { VolSongs.Clear(); }

            List<QueSong> QueSongList = JsonConvert.DeserializeObject<List<QueSong>>(jsonstring);

            foreach (var i in QueSongList)
            {
                var _index = i.SongNo.ToString();
                var _name = i.Title;
                var _artist = i.Artist == null ? "Unknown" : i.Artist;
                var _album = i.Album == null ? "Unknown" : i.Album;
                var _cover = i.Pic.ToString() == null ? "" : i.Pic.ToString();
                var _songurl = i.Src.ToString();

                if (!VolSongs.Any(p => p.Index == _index))
                {
                    VolSongs.Add(new LuoVolSong(VolNum, _index, _name, _artist, _album, _cover, _songurl));
                }
            }
            //check if there is repeat volSongs

            IsDetailGet = true;
        }


        public string GetVolSongFileJson(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var script = doc.DocumentNode.Descendants("script").ToArray().First(p => p.OuterHtml.Contains("var player")).OuterHtml;

            int b1 = script.IndexOf("[");//找a的位置
            int b2 = script.LastIndexOf("]");//找b的位置
            var jsonString = (script.Substring(b1)).Substring(0, b2 - b1 + 1);
            return jsonString;
        }

        public string GetVolSongSource(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var script = doc.DocumentNode.Descendants("script").ToArray().First(p => p.OuterHtml.Contains("var player")).OuterHtml;
            var SourceNum = "";
            if (script != null)
            {
                SourceNum = Regex.Replace(script, @"[^\d.\d]", "");
                return SourceNum;
            }
            else
                return null;
        }

        public void AddSongsFromJson(string songJson)
        {
            if (VolSongs == null)
            { VolSongs = new ObservableCollection<LuoVolSong>(); }
            else
            { VolSongs.Clear(); }

            var list = WashaSong.FromJson(songJson).Tracks;

            foreach (var i in list)
            {
                var _index = string.Format("{0:D2}", list.IndexOf(i) + 1);
                var _name = i.Title;
                var _artist = i.Meta == null ? "Unknown" : i.Meta.Artist;
                var _album = i.Meta == null ? "Unknown" : i.Meta.Album;
                var _cover = i.Image == null ? "" : i.Image.Src;
                var _songurl = i.Src;

                if (!VolSongs.Any(p => p.Index == _index))
                {
                    VolSongs.Add(new LuoVolSong(VolNum, _index, _name, _artist, _album, _cover, _songurl));
                }
            }



            //var list = SimpleSong.FromJson(songJson);

            //if(IsVolSongOnline)
            //{
            //    foreach (var i in list)
            //    {
            //        var _index = string.Format("{0:D2}", list.IndexOf(i)+1);
            //        var _name = i.Name;
            //        var _artist = i.Author;
            //        var _cover = i.Cover;
            //        var _songid = i.SongId;

            //        if (!VolSongs.Any(p => p.Index == _index))
            //        {
            //            VolSongs.Add(new LuoVolSong(VolNum, _index, _name, _artist, _cover, _songid));
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var i in list)
            //    {
            //        var _index = i.Name.Substring(0, 2);
            //        var _name = i.Name.Substring(4);
            //        var _artist = i.Author;
            //        var _cover = Cover;
            //        var _songsrc = i.Src.OriginalString;

            //        if(!VolSongs.Any(p => p.Index == _index))
            //        {
            //            VolSongs.Add(new LuoVolSong() {
            //                VolNum = VolNum,
            //                Index = _index,
            //                Name = _name,
            //                Artist = _artist,
            //                Album = "",
            //                AlbumImage = _cover,
            //                SongUrl = _songsrc
            //            });
            //        }
            //    }
            //}


        }

        //Get WashaVolDetailPageUri from WashaVolHtml
        public string GetWashaVolDetailUri(string washaVolPageHtml)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(washaVolPageHtml);

            //var vollist = new ObservableCollection<LuoVol>();


            //look for the firstNode
            var firstNode = doc.DocumentNode.SelectSingleNode("//a[@class='big-archive blog-post-image-link-block w-inline-block']");
            if (firstNode.GetAttributeValue("title", "").Contains(VolNum))
            { return firstNode.GetAttributeValue("href", ""); }

            //look fir the listNode
            var list = doc.DocumentNode.SelectNodes("//div[@class='blog-post-item w-col w-col-4 w-dyn-item']");

            foreach (var i in list)
            {
                var title = i.SelectSingleNode("./a").GetAttributeValue("title", "");
                if (title.Contains(VolNum))
                { return i.SelectSingleNode("./a").GetAttributeValue("href", ""); }
            }
            return null;
        }

        //Get WashaVolSongListJson from washaVolDetailPageHtml
        public string GetWashaVolSongListJson(string washaVolDetailPageHtml)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(washaVolDetailPageHtml);
            var JsonNode = doc.DocumentNode.SelectSingleNode("//script[@class='wp-playlist-script']");
            return JsonNode.InnerHtml.Replace("[]", "\"\"");
        }
    }
}
