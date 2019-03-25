using HtmlAgilityPack;
using Luo.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public bool IsDetailGet = false;

        private ObservableCollection<LuoVolSong> volSongs;
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

        public LuoVol(string _cover,string _volnum, string _volurl, string _title, string _commentcount, string _favdcount)
        {
            Cover = _cover;
            VolNum = _volnum;
            VolUrl = _volurl;
            Title = _title;
            CommentCount = _commentcount;
            FavdCount = _favdcount;
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
    }
}
