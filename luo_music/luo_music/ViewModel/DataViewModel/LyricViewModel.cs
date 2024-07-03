using GalaSoft.MvvmLight;
using Luo.Shared.Data;
using Luo.Shared.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;

namespace LuoMusic.ViewModel.DataViewModel
{
    public class LyricViewModel : ViewModelBase
    {
        protected VolService _volService;

        private ObservableCollection<LrcContent> content;
        public ObservableCollection<LrcContent> Contents
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                RaisePropertyChanged(() => Contents);
            }

        }
        private int index = -1;
        public int CurrentIndex
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                RaisePropertyChanged(() => CurrentIndex);
            }

        }

        private bool hasLryic;
        public bool HasLyric
        {
            get
            {
                return hasLryic;
            }
            set
            {
                hasLryic = value;
                RaisePropertyChanged(() => HasLyric);
            }

        }

        public string GetCurrent(int p)
        {
            if (CurrentIndex < Contents.Count && CurrentIndex > -1)
            {
                return Contents[CurrentIndex].Content;
            }
            return "NoLyricText";
        }


        public string GetPrevious(int p)
        {
            if (CurrentIndex < Contents.Count && CurrentIndex > 0)
            {
                return Contents[CurrentIndex - 1].Content;
            }
            return string.Empty;
        }


        public string GetNext(int p)
        {
            if (CurrentIndex < Contents.Count - 1 && CurrentIndex > -1)
            {
                return Contents[CurrentIndex + 1].Content;
            }
            return "EndText";
        }

        private Lyrics lyric;

        public Lyrics Lyric { get => lyric; }

        public LyricViewModel()
        {
            Contents = new ObservableCollection<LrcContent>();
            _volService = new VolService("", new LuoVolFactory(), CancellationTokenSourceFactory.CreateDefault());
        }

        public async void LoadLrcAsync(string lrcurl,TimeSpan TotalDuration)
        {
            var lrcfile=await _volService.GetLrcStringAsync(lrcurl);
            if (lrcfile != null)
            {
                Clear();
                var l = new Lyrics(lrcfile, TotalDuration);
                New(l);
            }
        }

        public void Clear()
        {
            lock (Contents)
            {
                CurrentIndex = -1;
                Contents.Clear();
                lyric = null;
                HasLyric = false;
            }
        }

        public void New(Lyrics l)
        {
            lock (Contents)
            {
                CurrentIndex = -1;
                Contents.Clear();
                if (l == null || l == default(Lyrics))
                {
                    HasLyric = false;
                    return;
                }
                lyric = l;
                foreach (var item in l)
                {
                    Contents.Add(new LrcContent() { Content = item.Value });
                }
                HasLyric = true;
            }
        }

        private int lastIndex = -1;

        public void Update(TimeSpan current)
        {
            lock (Contents)
            {
                var currentIndex = lastIndex;

                if (lyric == null || Contents.Count == 0)
                {
                    return;
                }
                bool b = false;
                for (int i = 0; i < lyric.Count; i++)
                {
                    if (!b && current < (lyric[i].Key + lyric.Offset))
                    {
                        if (i == 0)
                        {
                            currentIndex = 0;
                        }
                        else
                        {
                            currentIndex = i - 1;
                        }
                        b = true;
                    }
                }
                if (!b)
                {
                    currentIndex = lyric.Count - 1;
                }


                if (currentIndex == lastIndex)
                {

                }
                else
                {
                    CurrentIndex = currentIndex;
                    lastIndex = currentIndex;

                    for (int i = 0; i < lyric.Count; i++)
                    {
                        Contents[i].IsCurrent = false;
                        if (i == currentIndex)
                        {
                            Contents[i].IsCurrent = true;
                        }
                    }
                }
            }
        }
    }

    public class LrcContent : ViewModelBase
    {
        public string Content { get; set; }
        private bool isCurrent;
        public bool IsCurrent
        {
            get
            {
                return isCurrent;
            }
            set
            {
                isCurrent = value;
                RaisePropertyChanged(() => IsCurrent);
            }

        }

        public double CurrentOpacity(bool b)
        {
            return b ? 1d : 0.4d;
        }

        public FontWeight CurrentWeight(bool b)
        {
            return b ? FontWeights.Bold : FontWeights.Normal;
        }
    }
}
