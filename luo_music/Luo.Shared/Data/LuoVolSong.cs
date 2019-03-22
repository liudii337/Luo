using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

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

        public LuoVolSong(string _vol,string _index, string _name, string _artist, string _album, string _albumimage)
        {
            VolNum = _vol;
            Index = _index;
            Name = _name;
            Artist = _artist;
            Album = _album;
            AlbumImage = _albumimage;
            SongUrl = SongUriFormat(_vol, _index);
        }

        private string SongUriFormat(string _vol, string _index)
        {
            return string.Format("http://192.168.73.133/mp3-cdn2.luoo.net/low/luoo/radio{0}/{1}.mp3", _vol, _index);
        }
    }
}
