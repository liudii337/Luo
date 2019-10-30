using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using luo_music.Model;
using System.Collections.ObjectModel;
using Luo.Shared.Data;
using luo_music.ViewModel.DataViewModel;
using Luo.Shared.Service;
using Luo.Shared.Helper;
using Luo.Shared.PlaybackEngine;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.System;
using luo_music.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using System.Runtime.Serialization;

namespace luo_music.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private string _clock = "Starting...";
        private int _counter;
        private RelayCommand _incrementCommand;
        private RelayCommand<string> _navigateCommand;
        private bool _runClock;
        private RelayCommand _sendMessageCommand;
        private RelayCommand _showDialogCommand;
        private string _welcomeTitle = string.Empty;
        public IPlayer player;

        #region Previous
        public string Clock
        {
            get
            {
                return _clock;
            }
            set
            {
                Set(ref _clock, value);
            }
        }

        public RelayCommand IncrementCommand
        {
            get
            {
                return _incrementCommand
                    ?? (_incrementCommand = new RelayCommand(
                    () =>
                    {
                        WelcomeTitle = string.Format("Counter clicked {0} times", ++_counter);
                    }));
            }
        }

        public RelayCommand<string> NavigateCommand
        {
            get
            {
                return _navigateCommand
                       ?? (_navigateCommand = new RelayCommand<string>(
                           p => _navigationService.NavigateTo(ViewModelLocator.SecondPageKey, p),
                           p => !string.IsNullOrEmpty(p)));
            }
        }

        public RelayCommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand
                    ?? (_sendMessageCommand = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(
                            new NotificationMessageAction<string>(
                                "Testing",
                                reply =>
                                {
                                    WelcomeTitle = reply;
                                }));
                    }));
            }
        }

        public RelayCommand ShowDialogCommand
        {
            get
            {
                return _showDialogCommand
                       ?? (_showDialogCommand = new RelayCommand(
                           async () =>
                           {
                               var dialog = ServiceLocator.Current.GetInstance<IDialogService>();
                               await dialog.ShowMessage("Hello Universal Application", "it works...");
                           }));
            }
        }

        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        public void RunClock()
        {
            _runClock = true;

            Task.Run(async () =>
            {
                while (_runClock)
                {
                    try
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            Clock = DateTime.Now.ToString("HH:mm:ss");
                        });

                        await Task.Delay(1000);
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        public void StopClock()
        {
            _runClock = false;
        }
        #endregion

        private CancellationTokenSourceFactory _ctsFactory;
        public CancellationTokenSourceFactory CtsFactory
        {
            get
            {
                return _ctsFactory ?? (_ctsFactory = CancellationTokenSourceFactory.CreateDefault());
            }
        }

        private LuoVolFactory _normalFactory;
        public LuoVolFactory NormalFactory
        {
            get
            {
                return _normalFactory ?? (_normalFactory = new LuoVolFactory());
            }
        }

        public MainViewModel(
            IDataService dataService,
            INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            Initialize();

            DataVM = new VolDataViewModel(this,
                new VolService(Request.GetAllVol_w, NormalFactory, CtsFactory));

            LuoVolTags = LuoVolFactory.GetVolTagList();
            DataVM.RefreshAsync();

            player = new Player();
            PlayMode = 0;

            Task.Run(() =>
            {
                player.DownloadProgressChanged += Player_DownloadProgressChanged;
                player.ItemsChanged += Player_StatusChanged;
                player.PlaybackStatusChanged += Player_PlaybackStatusChanged;
                player.PositionUpdated += Player_PositionUpdated;
            });
        }



        private async Task Initialize()
        {
            try
            {
                LuoVols = await _dataService.GetVolList();
                var item = await _dataService.GetData();
                WelcomeTitle = item.Title;
            }
            catch (Exception ex)
            {
                // Report error here
                WelcomeTitle = ex.Message;
            }


        }



        private ObservableCollection<LuoVol> _luoVols;
        public ObservableCollection<LuoVol> LuoVols
        {
            get
            {
                return _luoVols;
            }
            set
            {
                if (_luoVols != value)
                {
                    _luoVols = value;
                    RaisePropertyChanged(() => LuoVols);
                }
            }
        }

        private VolDataViewModel _dataVM;
        public VolDataViewModel DataVM
        {
            get
            {
                return _dataVM;
            }
            set
            {
                if (_dataVM != value)
                {
                    _dataVM = value;
                    RaisePropertyChanged(() => DataVM);
                }
            }
        }

        private VolDataViewModel _tagDataVM;
        public VolDataViewModel TagDataVM
        {
            get
            {
                return _tagDataVM;
            }
            set
            {
                if (_tagDataVM != value)
                {
                    _tagDataVM = value;
                    RaisePropertyChanged(() => TagDataVM);
                }
            }
        }

        private VolItem _currentVol;
        public VolItem CurrentVol
        {
            get
            {
                return _currentVol;
            }
            set
            {
                if (_currentVol != value)
                {
                    _currentVol = value;
                    RaisePropertyChanged(() => CurrentVol);
                }
            }
        }

        private LuoVolSong _currentSong;
        public LuoVolSong CurrentSong
        {
            get
            {
                return _currentSong;
            }
            set
            {
                if (_currentSong != value)
                {
                    var lastsong = _currentSong;
                    if(lastsong!=null)
                    {
                        lastsong.IsPlaying = false;
                    }

                    _currentSong = value;
                    _currentSong.IsPlaying = true;
                    RaisePropertyChanged(() => CurrentSong);
                }
            }
        }

        private int _currentIndex = -1;
        public int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
            set
            {
                if (_currentIndex != value)
                {

                    RaisePropertyChanged(() => CurrentIndex);
                }
            }
        }

        #region CategoryPage

        private ObservableCollection<LuoVolTag> _luoVolTags;
        public ObservableCollection<LuoVolTag> LuoVolTags
        {
            get
            {
                return _luoVolTags;
            }
            set
            {
                if (_luoVolTags != value)
                {
                    _luoVolTags = value;
                    RaisePropertyChanged(() => LuoVolTags);
                }
            }
        }

        private int _currentTagIndex = -1;
        public int CurrentTagIndex
        {
            get
            {
                return _currentTagIndex;
            }
            set
            {
                if (_currentTagIndex != value)
                {

                    _currentTagIndex = value;

                    if (_currentTagIndex != -1)
                    {
                        TagDataVM = new VolDataViewModel(this,
                            new VolService(Request.GetTagVol(LuoVolTags[_currentTagIndex].KeySrc), NormalFactory, CtsFactory));
                    }

                    RaisePropertyChanged(() => CurrentTagIndex);

                    if (TagDataVM != null)
                    {
                        TagDataVM.RefreshAsync();
                    }
                }
            }
        }
        #endregion

        #region Navigate
        public List<HamPanelItem> HamList { get; set; } = new List<HamPanelItem>()
        {
            new HamPanelItem
            {
                //多语言支持
                //Title = Consts.Localizer.GetString("HomeText"),
                Title="最新期刊",
                TargetType = typeof(VolListPage),
                Icon="\uE80F",
                Index = VirtualKey.Number1,
                IndexNum = "1"
            },
            new HamPanelItem
            {
                Title = "分类",
                Icon="\uE2AC",
                TargetType = typeof(CategoryPage),
                Index = VirtualKey.Number2,
                IndexNum = "2"
            },
            new HamPanelItem
            {
                Title = "收藏",
                Icon = "\uEFA9",
                TargetType = typeof(HeartPage),
                Index = VirtualKey.Number3,
                IndexNum = "3"
            },
        };

        private bool _needShowBack=false;
        public bool NeedShowBack
        {
            get
            {
                return _needShowBack;
            }
            set
            {
                if (_needShowBack != value)
                {
                    _needShowBack = value;
                    RaisePropertyChanged(() => NeedShowBack);
                }
            }
        }
        #endregion

        #region PlayMusic
        private bool _isPlaying;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    RaisePropertyChanged(() => IsPlaying);
                }
            }
        }

        private bool _isLoop;
        public bool IsLoop
        {
            get
            {
                return _isLoop;
            }
            set
            {
                if (_isLoop != value)
                {
                    _isLoop = value;
                    player?.Loop(value);
                    RaisePropertyChanged(() => IsLoop);                    
                }
            }
        }

        private bool _isOneLoop;
        public bool IsOneLoop
        {
            get
            {
                return _isOneLoop;
            }
            set
            {
                if (_isOneLoop != value)
                {
                    _isOneLoop = value;
                    player?.LoopOne(value);
                    RaisePropertyChanged(() => IsOneLoop);
                }
            }
        }

        private bool _isShuffle;
        public bool IsShuffle
        {
            get
            {
                return _isShuffle;
            }
            set
            {
                if (_isShuffle != value)
                {
                    _isShuffle = value;
                    player?.Shuffle(value);
                    RaisePropertyChanged(() => IsShuffle);
                }
            }
        }

        private int _playMode;
        public int PlayMode
        {
            get
            {
                return _playMode;
            }
            set
            {
                if (_playMode != value)
                {
                    _playMode = value;
                    RaisePropertyChanged(() => PlayMode);
                }
                switch (_playMode)
                {
                    case 0:
                        IsLoop = true;
                        IsOneLoop = false;
                        IsShuffle = false;
                        break;
                    case 1:
                        IsLoop = true;
                        IsOneLoop = true;
                        IsShuffle = false;
                        break;
                    case 2:
                        IsLoop = true;
                        IsOneLoop = false;
                        IsShuffle = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public Visibility VisibilityTrans(bool s)
        {
            return s == true ? Visibility.Collapsed : Visibility.Visible;
        }

        public string PlayPauseToIcon(bool isplaying)
        {
            return isplaying == true ? "\uE103" : "\uE102";
        }

        public string PlayModeToIcon(int playmode)
        {
            switch (playmode)
            {
                case 0:
                    return "\uE1CD";
                case 1:
                    return "\uE1CC";
                case 2:
                    return "\uE8B1";
                default:
                    return "\uE1CD";
            }
        }


        private double _bufferProgress;
        public double BufferProgress
        {
            get
            {
                return _bufferProgress;
            }
            set
            {
                if (_bufferProgress != value)
                {
                    _bufferProgress = value;
                    RaisePropertyChanged(() => BufferProgress);
                }
            }
        }

        private TimeSpan _currentPosition;
        public TimeSpan CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                if (_currentPosition != value)
                {
                    _currentPosition = value;
                    RaisePropertyChanged(() => CurrentPosition);
                }
            }
        }

        private TimeSpan _totalDuration;
        public TimeSpan TotalDuration
        {
            get
            {
                return _totalDuration;
            }
            set
            {
                if (_totalDuration != value)
                {
                    _totalDuration = value;
                    RaisePropertyChanged(() => TotalDuration);
                }
            }
        }

        public double PositionToValue(TimeSpan t1, TimeSpan total)
        {
            if (total == null || total == default(TimeSpan))
            {
                return 0;
            }
            return 100 * (t1.TotalMilliseconds / total.TotalMilliseconds);
        }

        internal void ChangePlayPosition(TimeSpan timeSpan)
        {
            if (timeSpan < TotalDuration)
            {
                player.Seek(timeSpan);
            }
        }

        public RelayCommand GoPrevious
        {
            get
            {
                return new RelayCommand(() =>
                {
                    player?.Previous();
                });
            }
        }

        public RelayCommand GoNext
        {
            get
            {
                return new RelayCommand(() =>
                {
                    player?.Next();
                });
            }
        }

        public RelayCommand Stop
        {
            get
            {
                return new RelayCommand(() =>
                {
                    player?.Stop();
                });
            }
        }

        public RelayCommand PlayPause
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (IsPlaying is bool b)
                    {
                        if (b)
                        {                            
                            player?.Pause();
                        }
                        else
                        {
                            player?.Play();
                        }
                    }
                    else
                    {
                        player?.Play();
                    }
                });
            }
        }

        public RelayCommand NextPlayMode
        {
            get
            {
                return new RelayCommand(() =>
                {
                    PlayMode++;
                    if(PlayMode==3)
                    { PlayMode = 0; }
                });
            }
        }

        internal async Task InstantPlayAsync(IList<LuoVolSong> songs, int startIndex = 0)
        {
            await player.NewPlayList(songs, startIndex);
            player.Play();
        }

        private async void Player_PositionUpdated(object sender, PositionUpdatedArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                CurrentPosition = e.Current;
                TotalDuration = e.Total;
            });
        }

        private async void Player_PlaybackStatusChanged(object sender, PlaybackStatusChangedArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                IsPlaying = e.PlaybackStatus == MediaPlaybackState.Playing;
                IsLoop = e.IsLoop;
                IsOneLoop = e.IsOneLoop;
                //IsShuffle = e.IsShuffle;



                //if (e.PlaybackStatus == MediaPlaybackState.Playing && Settings.Current.PreventLockscreen)
                //{
                //    ActivateDisplay();
                //}
            });
        }

        private async void Player_StatusChanged(object sender, PlayingItemsChangedArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                if (e.CurrentIndex == -1)
                {
                    //CurrentSong = null;
                    //NowPlayingList.Clear();
                    //NowListPreview = "-/-";
                    //CurrentTitle = null;
                    //CurrentAlbum = null;
                    //CurrentArtist = null;
                    //await CurrentArtwork.SetSourceAsync(await RandomAccessStreamReference.CreateFromUri(new Uri(Consts.BlackPlaceholder)).OpenReadAsync());
                    //CurrentIndex = -1;
                    //NeedShowPanel = false;
                    //IsPodcast = false;
                    //ReleaseDisplay();
                    return;
                }

                if (e.CurrentSong != null)
                {
                    CurrentSong = e.CurrentSong;
                    //CurrentSong.IsPlaying = true;
                    //var p = e.CurrentSong;
                    ////CurrentTitle = p.Title.IsNullorEmpty() ? p.FilePath.Split('\\').LastOrDefault() : p.Title;
                    ////IsPodcast = p.IsPodcast;
                    ////CurrentAlbum = p.Album.IsNullorEmpty() ? Consts.UnknownAlbum : p.Album;
                    ////CurrentArtist = p.Performers == null ? (p.AlbumArtists == null ? Consts.UnknownArtists : string.Join(Consts.CommaSeparator, p.AlbumArtists)) : string.Join(Consts.CommaSeparator, p.Performers);

                    //if (e.Thumnail != null)
                    //{
                    //    await CurrentArtwork.SetSourceAsync(await e.Thumnail.OpenReadAsync());
                    //}
                    //else
                    //{
                    //    var thumb = RandomAccessStreamReference.CreateFromUri(new Uri(Consts.BlackPlaceholder));
                    //    await CurrentArtwork.SetSourceAsync(await thumb.OpenReadAsync());
                    //}

                    //var task = Task.Run(() =>
                    //{
                    //    Tile.SendNormal(CurrentTitle, CurrentAlbum, string.Join(Consts.CommaSeparator, p.Performers ?? new string[] { }), p.PicturePath);
                    //});

                }
                //if (e.Items is IReadOnlyList<Song> l)
                //{
                //    NowListPreview = $"{e.CurrentIndex + 1}/{l.Count}";
                //    NowPlayingList.Clear();
                //    for (int i = 0; i < l.Count; i++)
                //    {
                //        NowPlayingList.Add(new SongViewModel(l[i])
                //        {
                //            Index = (uint)i,
                //        });
                //    }
                //}
                CurrentIndex = e.CurrentIndex;

//                ApplicationView.GetForCurrentView().Title = CurrentPlayingDesc();

//                if (e.CurrentSong != null && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Shell.AdaptiveCardBuilder"))
//                {
//                    var last = NowPlayingList.Count - 1 <= currentIndex;

//                    string img0, img1;
//                    img1 = null;

//                    if (!NowPlayingList[currentIndex].IsOnline)
//                    {
//                                    //if (NowPlayingList[currentIndex].Song.PicturePath.IsNullorEmpty())
//                                    //{
//                                    //    img0 = Consts.BlackPlaceholder;
//                                    //}
//                                    //else
//                                    //{
//                                    //    img0 = $"ms-appdata:///temp/{NowPlayingList[currentIndex].Artwork.AbsoluteUri.Split('/').Last()}";
//                                    //}
//                                    img0 = null;
//                    }
//                    else
//                    {
//                        img0 = NowPlayingList[currentIndex].Artwork?.AbsoluteUri;
//                    }

//                    var otherArtwork = NowPlayingList.Where(a => a.Artwork?.AbsoluteUri != img0);

//                    foreach (var item in otherArtwork)
//                    {
//                        if (!item.IsOnline)
//                        {
//                            img1 = null;
//                        }
//                        else
//                        {
//                            img1 = item.Artwork.AbsoluteUri;
//                        }
//                        break;
//                    }

//                    var json = await TimelineCard.AuthorAsync(currentTitle, currentAlbum, currentArtist, img0, img1, NowPlayingList.Count);

//                    act.ActivationUri = new Uri("as-music:///?action=timeline-restore");

//                    act.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(json);
//                    act.VisualElements.DisplayText = Consts.Localizer.GetString("AppNameText");
//                    act.VisualElements.Description = Consts.Localizer.GetString("TimelineTitle");
//                    await act.SaveAsync();

//                    var songs = NowPlayingList.Where(s => s.IsOnedrive || s.IsOnline).Select(s => s.Song).ToList();
//#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
//                                Task.Run(async () =>
//                    {
//                        if (songs.Count > 0)
//                        {
//                            var status = new PlayerStatus(songs, currentIndex, currentPosition);
//                            await status.RoamingSaveAsync();
//                        }
//                        else
//                        {
//                            await PlayerStatus.ClearRoamingAsync();
//                        }
//                    });
//#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

//                                await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
//                    {
//                        try
//                        {
//                                        //Dispose of any current UserActivitySession, and create a new one.
//                                        (_currentActivity as UserActivitySession)?.Dispose();
//                            _currentActivity = act.CreateSession();
//                        }
//                        catch (Exception)
//                        {
//                        }
//                    });
//                }
            });
        }

        private async void Player_DownloadProgressChanged(object sender, DownloadProgressChangedArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                BufferProgress = 100 * e.Progress;
            });
        }

        #endregion
    }
}