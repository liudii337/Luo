using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private string[] tags;
        public string[] Tags
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

    }
}
