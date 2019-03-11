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
        private IPlayer player;

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
                new VolService(Request.GetAllVol, NormalFactory, CtsFactory));

            DataVM.RefreshAsync();

            player = new Player();

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
                    RaisePropertyChanged(nameof(CurrentVol));
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
                    _currentSong = value;
                    RaisePropertyChanged(() => CurrentSong);
                }
            }
        }

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

        internal async Task InstantPlayAsync(IList<LuoVolSong> songs, int startIndex = 0)
        {
            await player.NewPlayList(songs, startIndex);
            player.Play();
        }

        private void Player_PositionUpdated(object sender, PositionUpdatedArgs e)
        {
            //throw new NotImplementedException();
        }

        private async void Player_PlaybackStatusChanged(object sender, PlaybackStatusChangedArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                IsPlaying = e.PlaybackStatus == MediaPlaybackState.Playing;
                //isLoop = e.IsLoop;
                //isShuffle = e.IsShuffle;
                //RaisePropertyChanged("IsLoop");
                //RaisePropertyChanged("IsShuffle");

                //if (e.PlaybackStatus == MediaPlaybackState.Playing && Settings.Current.PreventLockscreen)
                //{
                //    ActivateDisplay();
                //}
            });
        }

        private async void Player_StatusChanged(object sender, PlayingItemsChangedArgs e)
        {
//            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
//            {
//                if (e.CurrentIndex == -1)
//                {
//                    CurrentSong = null;
//                    //NowPlayingList.Clear();
//                    //NowListPreview = "-/-";
//                    //CurrentTitle = null;
//                    //CurrentAlbum = null;
//                    //CurrentArtist = null;
//                    //await CurrentArtwork.SetSourceAsync(await RandomAccessStreamReference.CreateFromUri(new Uri(Consts.BlackPlaceholder)).OpenReadAsync());
//                    //CurrentIndex = -1;
//                    //NeedShowPanel = false;
//                    //IsPodcast = false;
//                    //ReleaseDisplay();
//                    return;
//                }

//                if (e.CurrentSong != null)
//                {
//                    CurrentSong = e.CurrentSong;
//                    var p = e.CurrentSong;
//                    //CurrentTitle = p.Title.IsNullorEmpty() ? p.FilePath.Split('\\').LastOrDefault() : p.Title;
//                    //IsPodcast = p.IsPodcast;
//                    //CurrentAlbum = p.Album.IsNullorEmpty() ? Consts.UnknownAlbum : p.Album;
//                    //CurrentArtist = p.Performers == null ? (p.AlbumArtists == null ? Consts.UnknownArtists : string.Join(Consts.CommaSeparator, p.AlbumArtists)) : string.Join(Consts.CommaSeparator, p.Performers);

//                    if (e.Thumnail != null)
//                    {
//                        await CurrentArtwork.SetSourceAsync(await e.Thumnail.OpenReadAsync());
//                    }
//                    else
//                    {
//                        var thumb = RandomAccessStreamReference.CreateFromUri(new Uri(Consts.BlackPlaceholder));
//                        await CurrentArtwork.SetSourceAsync(await thumb.OpenReadAsync());
//                    }

//                    var task = Task.Run(() =>
//                    {
//                        Tile.SendNormal(CurrentTitle, CurrentAlbum, string.Join(Consts.CommaSeparator, p.Performers ?? new string[] { }), p.PicturePath);
//                    });

//                }
//                if (e.Items is IReadOnlyList<Song> l)
//                {
//                    NowListPreview = $"{e.CurrentIndex + 1}/{l.Count}";
//                    NowPlayingList.Clear();
//                    for (int i = 0; i < l.Count; i++)
//                    {
//                        NowPlayingList.Add(new SongViewModel(l[i])
//                        {
//                            Index = (uint)i,
//                        });
//                    }
//                }
//                if (e.CurrentIndex < NowPlayingList.Count)
//                {
//                    CurrentIndex = e.CurrentIndex;

//                }
//                if (MainPage.Current.IsCurrentDouban)
//                {
//                    return;
//                }
//                else
//                {
//                    NeedShowPanel = true;
//                }
//                ApplicationView.GetForCurrentView().Title = CurrentPlayingDesc();

//                if (e.CurrentSong != null && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Shell.AdaptiveCardBuilder"))
//                {
//                    var last = NowPlayingList.Count - 1 <= currentIndex;

//                    string img0, img1;
//                    img1 = null;

//                    if (!NowPlayingList[currentIndex].IsOnline)
//                    {
//                        //if (NowPlayingList[currentIndex].Song.PicturePath.IsNullorEmpty())
//                        //{
//                        //    img0 = Consts.BlackPlaceholder;
//                        //}
//                        //else
//                        //{
//                        //    img0 = $"ms-appdata:///temp/{NowPlayingList[currentIndex].Artwork.AbsoluteUri.Split('/').Last()}";
//                        //}
//                        img0 = null;
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
//                    Task.Run(async () =>
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

//                    await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
//                    {
//                        try
//                        {
//                            //Dispose of any current UserActivitySession, and create a new one.
//                            (_currentActivity as UserActivitySession)?.Dispose();
//                            _currentActivity = act.CreateSession();
//                        }
//                        catch (Exception)
//                        {
//                        }
//                    });
//                }
//            });
        }

        private void Player_DownloadProgressChanged(object sender, DownloadProgressChangedArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}