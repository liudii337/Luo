using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Luo.Shared.Data
{
    public class LuoVolSong : ModelBase
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

        private string songId;
        public string SongId
        {
            get
            {
                return songId;
            }
            set
            {
                if (songId != value)
                {
                    songId = value;
                    RaisePropertyChanged(() => SongId);
                }
            }
        }

        private string index;
        public string Index
        {
            get
            {
                return index;
            }
            set
            {
                if (index != value)
                {
                    index = value;
                    RaisePropertyChanged(() => Index);
                }
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        private string artist;
        public string Artist
        {
            get
            {
                return artist;
            }
            set
            {
                if (artist != value)
                {
                    artist = value;
                    RaisePropertyChanged(() => Artist);
                }
            }
        }

        private string album;
        public string Album
        {
            get
            {
                return album;
            }
            set
            {
                if (album != value)
                {
                    album = value;
                    RaisePropertyChanged(() => Album);
                }
            }
        }

        private string albumimage;
        public string AlbumImage
        {
            get
            {
                return albumimage;
            }
            set
            {
                if (albumimage != value)
                {
                    albumimage = value;
                    RaisePropertyChanged(() => AlbumImage);
                }
            }
        }

        private string songurl;
        public string SongUrl
        {
            get
            {
                return songurl;
            }
            set
            {
                if (songurl != value)
                {
                    songurl = value;
                    RaisePropertyChanged(() => SongUrl);
                }
            }
        }

        public bool IsOnline = true;

        private bool isPlaying = false;
        [IgnoreDataMember]
        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                    RaisePropertyChanged(() => IsPlaying);
                }
            }
        }

        public Visibility VisibilityTrans(bool s)
        {
            return s == true ? Visibility.Collapsed : Visibility.Visible;
        }

        public Brush ColorTrans(bool s)
        {
            bool IsLight = true;
            if (Window.Current.Content is FrameworkElement rootElement)
            {
                // If the user switch to follow the system, then we apply the App's theme instead of element's theme.
                if (rootElement.RequestedTheme == ElementTheme.Default)
                {
                    IsLight = Application.Current.RequestedTheme == ApplicationTheme.Light;
                }
                else
                {
                    IsLight = rootElement.RequestedTheme == ElementTheme.Light;
                }
            }
            return s == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush((IsLight ?  Colors.Black : Colors.White));
        }

        public LuoVolSong()
        {
        }

        //public LuoVolSong(string _vol,string _index, string _name, string _artist, string _album, string _albumimage)
        //{
        //    VolNum = _vol;
        //    Index = _index;
        //    Name = _name;
        //    Artist = _artist;
        //    Album = _album;
        //    AlbumImage = _albumimage;
        //    SongUrl = SongUriFormat(_vol, _index);
        //}

        //private string SongUriFormat(string _vol, string _index)
        //{
        //    var vol = int.Parse(_vol);
        //    return string.Format("http://mp3-cdn2.luoo.net/low/luoo/radio{0}/{1}.mp3", vol, _index);
        //}

        public LuoVolSong(string _vol, string _index, string _name, string _artist, string _songurl)
        {
            VolNum = _vol;
            Index = _index;
            Name = _name;
            Artist = _artist;
            Album = "";
            AlbumImage = "";
            SongUrl = _songurl;
        }

        public LuoVolSong(string _vol, string _index, string _name, string _artist, string _cover, string _songid)
        {
            VolNum = _vol;
            Index = _index;
            Name = _name;
            Artist = _artist;
            Album = "";
            AlbumImage = _cover;
            SongId = _songid;
        }

        public LuoVolSong(string _vol, string _index, string _name, string _artist, string _album, string _cover, string _songurl)
        {
            VolNum = _vol;
            Index = _index;
            Name = _name;
            Artist = _artist;
            Album = _album;
            AlbumImage = _cover;
            SongUrl = _songurl;
        }

        //example: http://192.168.73.132/luoow.wxwenku.com/999/07._Froesche_Die_Schröders.mp3
        private string SongUriFormat_w(string _vol, string _index, string _name)
        {
            var vol = int.Parse(_vol);
            var name = _name.Replace(" ", "_");
            return string.Format("http://192.168.73.132/luoow.wxwenku.com/{0}/{1}._{2}.mp3", vol, _index, name);
        }
    }
}
