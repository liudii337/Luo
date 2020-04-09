using Luo.Shared.Data;
using Luo.Shared.Extension;
using Luo.Shared.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.Web.Http;

namespace Luo.Shared.PlaybackEngine
{
    public sealed class Player : IDisposable, IPlayer
    {
        private MediaPlaybackList mediaPlaybackList;

        private DeviceInformation _autoDevice;

        private readonly object lockable = new object();
        private int _songCountID;
        private IAsyncAction _addPlayListTask;
        private List<LuoVolSong> currentList;
        private bool newComing;
        private ThreadPoolTimer positionUpdateTimer;

        public bool? IsPlaying { get; private set; }
        private MediaPlaybackState _savedState;
        private TimeSpan _savedPosition;
        private uint _savedIndex;

        public MediaPlayer MediaPlayer { get; private set; }

        public double PlaybackRate
        {
            get => MediaPlayer.PlaybackSession.PlaybackRate;
            set => MediaPlayer.PlaybackSession.PlaybackRate = value;
        }


        private bool isShuffle;
        public bool? IsShuffle
        {
            get => isShuffle;
            set => isShuffle = value == true;
        }

        private List<LuoVolSong> currentListBackup;

        public double Volume => MediaPlayer.Volume;

        public event EventHandler<PositionUpdatedArgs> PositionUpdated;

        public async void ChangeAudioEndPoint(string outputDeviceID)
        {
            if (outputDeviceID.IsNullorEmpty() || outputDeviceID == MediaPlayer.AudioDevice?.Id)
            {
                return;
            }
            var outputDevice = await DeviceInformation.CreateFromIdAsync(outputDeviceID);
            MediaPlayer.AudioDevice = outputDevice;
            Pause();
        }

        public void ChangeVolume(double value)
        {
            MediaPlayer.Volume = value / 100d;
        }

        private async Task ToggleVolumeAnimation(bool istoplaying)
        {
            //淡入淡出效果
            var startVolume = istoplaying ? 0 : 100;
            var goalVolume = istoplaying ? 100 : 0;

            var step = (goalVolume - startVolume) / 20;
            do
            {
                startVolume = startVolume + step;
                ChangeVolume(startVolume);
                await Task.Delay(15);
            } while (startVolume != goalVolume);           
        }

        public event EventHandler<DownloadProgressChangedArgs> DownloadProgressChanged;
        public event EventHandler<PlaybackStatusChangedArgs> PlaybackStatusChanged;
        public event EventHandler<PlayingItemsChangedArgs> ItemsChanged;

        public Player()
        {
            currentList = new List<LuoVolSong>();

            InitMediaPlayer();

            mediaPlaybackList = new MediaPlaybackList();
            mediaPlaybackList.CurrentItemChanged += MediaPlaybackList_CurrentItemChanged;
            MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChangedAsync;
        }

        private void InitMediaPlayer()
        {
            MediaPlayer = new MediaPlayer
            {
                AudioCategory = MediaPlayerAudioCategory.Media,
            };
            //ChangeAudioEndPoint(Settings.Current.OutputDeviceID);
            //ChangeVolume(Settings.Current.PlayerVolume);

            _autoDevice = MediaPlayer.AudioDevice;
            var type = MediaPlayer.AudioDeviceType;
            var mgr = MediaPlayer.CommandManager;
            mgr.IsEnabled = true;
            positionUpdateTimer = ThreadPoolTimer.CreatePeriodicTimer(UpdatTimerHandler, TimeSpan.FromMilliseconds(250), UpdateTimerDestoyed);


            //if (Settings.Current.AudioGraphEffects.HasFlag(Core.Models.Effects.ChannelShift))
            //    MediaPlayer.AddAudioEffect(typeof(ChannelShift).FullName, false, new PropertySet()
            //    {
            //        ["Shift"] = Settings.Current.ChannelShift,
            //        ["Mono"] = Settings.Current.StereoToMono
            //    });
            //if (Settings.Current.AudioGraphEffects.HasFlag(Core.Models.Effects.Equalizer))
            //    MediaPlayer.AddAudioEffect(typeof(SuperEQ).FullName, false, new PropertySet()
            //    {
            //        ["EqualizerBand"] = new EqualizerBand[]
            //        {
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 30, Gain = Settings.Current.Gain[0]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 75, Gain = Settings.Current.Gain[1]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 150, Gain = Settings.Current.Gain[2]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 30, Gain = Settings.Current.Gain[3]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 600, Gain = Settings.Current.Gain[4]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 1250, Gain = Settings.Current.Gain[5]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 2500, Gain = Settings.Current.Gain[6]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 5000, Gain = Settings.Current.Gain[7]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 10000, Gain = Settings.Current.Gain[8]},
            //            new EqualizerBand {Bandwidth = 0.8f, Frequency = 20000, Gain = Settings.Current.Gain[9]},
            //        }
            //    });
            //if (Settings.Current.AudioGraphEffects.HasFlag(Core.Models.Effects.Limiter))
            //    MediaPlayer.AddAudioEffect(typeof(Threshold).FullName, false, new PropertySet()
            //    {
            //    });
        }

        private void UpdateTimerDestoyed(ThreadPoolTimer timer)
        {
            timer.Cancel();
            timer = null;
            positionUpdateTimer?.Cancel();
            positionUpdateTimer = null;
            positionUpdateTimer = ThreadPoolTimer.CreatePeriodicTimer(UpdatTimerHandler, TimeSpan.FromMilliseconds(250), UpdateTimerDestoyed);
        }

        private void UpdatTimerHandler(ThreadPoolTimer timer)
        {
            if (MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                PlaybackSession_PositionChangedAsync(MediaPlayer.PlaybackSession, null);
            }
        }

        private void PlaybackSession_PositionChangedAsync(MediaPlaybackSession sender, object args)
        {
            lock (lockable)
            {
                var updatedArgs = new PositionUpdatedArgs
                {
                    Current = sender.Position,
                    Total = MediaPlayer.PlaybackSession.NaturalDuration
                };
                PositionUpdated?.Invoke(this, updatedArgs);
                DownloadProgressChanged?.Invoke(this, new DownloadProgressChangedArgs()
                {
                    Progress = sender.DownloadProgress
                });
                try
                {
                    if (!(mediaPlaybackList.CurrentItem?.Source.CustomProperties["SONG"] is LuoVolSong song)) return;
                    if (song.IsOnline)
                    {
                        // TODO: Online Music Statistic
                        //throw new NotImplementedException("Play Statistic online");
                    }
                    else
                    {
                        //var id = song.ID;
                        //if (id != default(int) && _songCountID != id && updatedArgs.Current.TotalSeconds / updatedArgs.Total.TotalSeconds > 0.5)
                        //{
                        //    _songCountID = id;
                        //    var t = ThreadPool.RunAsync(async (x) =>
                        //    {
                        //        var opr = SQLOperator.Current();
                        //        await FileReader.PlayStaticAddAsync(id, 0, 1);
                        //    });
                        //}
                    }
                }
                catch (Exception)
                {

                }
            }
        }



        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            // TODO: When error, restore

            switch (MediaPlayer.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.None:
                case MediaPlaybackState.Opening:
                case MediaPlaybackState.Buffering:
                    IsPlaying = null;
                    break;
                case MediaPlaybackState.Playing:
                    IsPlaying = true;
                    break;
                case MediaPlaybackState.Paused:
                    IsPlaying = false;
                    break;
                default:
                    break;
            }

            PlaybackStatusChanged?.Invoke(this, new PlaybackStatusChangedArgs()
            {
                PlaybackStatus = MediaPlayer.PlaybackSession.PlaybackState,
                IsOneLoop=MediaPlayer.IsLoopingEnabled,
                IsLoop = mediaPlaybackList.AutoRepeatEnabled,
                IsShuffle = isShuffle
            });
        }

        private void MediaPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            switch (MediaPlayer.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.None:
                case MediaPlaybackState.Opening:
                case MediaPlaybackState.Buffering:
                    IsPlaying = null;
                    break;
                case MediaPlaybackState.Playing:
                    IsPlaying = true;
                    break;
                case MediaPlaybackState.Paused:
                    IsPlaying = false;
                    break;
                default:
                    break;
            }


            var currentSong = mediaPlaybackList.CurrentItem?.Source.CustomProperties["SONG"] as LuoVolSong;


            ItemsChanged?.Invoke(this, new PlayingItemsChangedArgs
            {
                IsShuffle = isShuffle,
                IsOneLoop = MediaPlayer.IsLoopingEnabled,
                IsLoop = mediaPlaybackList.AutoRepeatEnabled,
                CurrentSong = currentSong,
                CurrentIndex = currentSong == null ? -1 : currentList.FindIndex(a => a == currentSong),
                Items = currentList,
                Thumnail = mediaPlaybackList.CurrentItem?.GetDisplayProperties().Thumbnail
            });

            PlaybackSession_PositionChangedAsync(MediaPlayer.PlaybackSession, null);

            // TODO: restore when error
            if (args.Reason == MediaPlaybackItemChangedReason.Error)
            {
                throw new IOException("Play back error.");
            }
        }

        public async Task NewPlayList(IList<LuoVolSong> items, int startIndex = 0)
        {
            if (items.IsNullorEmpty())
            {
                return;
            }

            newComing = true;
            _addPlayListTask?.Cancel();

            MediaPlayer.Pause();
            mediaPlaybackList.Items.Clear();

            if (isShuffle)
            {
                currentListBackup = items.ToList();
                var item = items[startIndex];
                items.Shuffle();
                var i = items.IndexOf(item);
                var p = items[startIndex];
                items[startIndex] = items[i];
                items[i] = p;
            }

            currentList.Clear();
            currentList.AddRange(items);

            if (startIndex <= 0)
            {
                var item = items[0];
                var mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);

                while(mediaPlaybackItem == null)
                {
                    startIndex = startIndex++;
                    if(startIndex==items.Count)
                    { startIndex = 0; }
                    item = items[startIndex];
                    mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);
                }

                mediaPlaybackList.Items.Add(mediaPlaybackItem);
                mediaPlaybackList.StartingItem = mediaPlaybackItem;

                MediaPlayer.Source = mediaPlaybackList;

                newComing = false;

                _addPlayListTask = ThreadPool.RunAsync(async (x) =>
                {
                    await AddtoPlayListAsync(items.TakeLast(items.Count - 1));
                });
            }
            else
            {
                if (startIndex >= items.Count)
                {
                    startIndex = items.Count - 1;
                }
                var item = items[startIndex];
                var mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);

                while (mediaPlaybackItem == null)
                {
                    startIndex = startIndex + 1;
                    if (startIndex == items.Count)
                    { startIndex = 0; }
                    item = items[startIndex];
                    mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);
                }

                var listBefore = items.Take(startIndex);
                var listAfter = items.TakeLast(items.Count - 1 - startIndex);

                mediaPlaybackList.Items.Add(mediaPlaybackItem);
                mediaPlaybackList.StartingItem = mediaPlaybackItem;

                MediaPlayer.Source = mediaPlaybackList;

                newComing = false;

                _addPlayListTask = ThreadPool.RunAsync(async (x) =>
                {
                    var a = AddtoPlayListFirstAsync(listBefore);
                    var s = AddtoPlayListAsync(listAfter);
                    await Task.WhenAll(new Task[] { a, s });
                });
            }

            PlaybackSession_PlaybackStateChanged(null, null);
        }

        //public async Task NewPlayList(IList<StorageFile> items, int startIndex = 0)
        //{
        //    if (items.IsNullorEmpty())
        //    {
        //        return;
        //    }

        //    newComing = true;
        //    _addPlayListTask?.Cancel();

        //    MediaPlayer.Pause();
        //    mediaPlaybackList.Items.Clear();

        //    currentList.Clear();

        //    if (isShuffle)
        //    {
        //        //currentListBackup = items.ToList();
        //        var item = items[startIndex];
        //        items.Shuffle();
        //        var i = items.IndexOf(item);
        //        var p = items[startIndex];
        //        items[startIndex] = items[i];
        //        items[i] = p;
        //    }

        //    foreach (var file in items)
        //    {
        //        MediaSource mediaSource;

        //        var item = await FileReader.ReadFileAsync(file);

        //        mediaSource = MediaSource.CreateFromStorageFile(file);
        //        mediaSource.CustomProperties[Consts.SONG] = item;

        //        var mediaPlaybackItem = new MediaPlaybackItem(mediaSource);
        //        var props = mediaPlaybackItem.GetDisplayProperties();

        //        await WritePropertiesAsync(item, props, item.PicturePath);

        //        mediaPlaybackItem.ApplyDisplayProperties(props);
        //        mediaPlaybackList.Items.Add(mediaPlaybackItem);
        //        currentList.Add(item);
        //    }
        //    if (startIndex > mediaPlaybackList.Items.Count - 1)
        //    {
        //        startIndex = mediaPlaybackList.Items.Count - 1;
        //    }
        //    if (startIndex < 0)
        //        startIndex = 0;

        //    mediaPlaybackList.StartingItem = mediaPlaybackList.Items[startIndex];
        //    MediaPlayer.Source = mediaPlaybackList;
        //}

        private async Task AddtoPlayListFirstAsync(IEnumerable<LuoVolSong> items)
        {
            foreach (var item in items.Reverse())
            {
                try
                {
                    var mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);

                    if (newComing)
                        return;

                    if (mediaPlaybackItem == null)
                        continue;

                    mediaPlaybackList.Items.Insert(0, mediaPlaybackItem);
                }
                catch (FileNotFoundException)
                {
                    continue;
                }
            }
        }

        private async Task AddtoPlayListAsync(IEnumerable<LuoVolSong> items)
        {
            foreach (var item in items)
            {
                try
                {
                    var mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);

                    if (newComing)
                        return;

                    if (mediaPlaybackItem == null)
                        continue;

                    mediaPlaybackList.Items.Add(mediaPlaybackItem);
                }
                catch (FileNotFoundException)
                {
                    continue;
                }
            }
        }

        private async Task<MediaPlaybackItem> GetMediaPlaybackItemAsync(LuoVolSong item)
        {
            //if (item.IsOnline)
            //{

            //HttpClient client = new HttpClient();

            //client.DefaultRequestHeaders.Add("Cookie", CookieHelper.GetCookie());

            //HttpRandomAccessStream stream = await HttpRandomAccessStream.CreateAsync(client, new Uri(item.SongUrl));

            //var mediaSource = MediaSource.CreateFromStream(stream, stream.ContentType);

            ////AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(new Uri(item.SongUrl), client);

            ////if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
            ////{
            ////    mediaSource = MediaSource.CreateFromAdaptiveMediaSource( result.MediaSource;
            ////}
            //var httpmanager = new HttpCookieManager();
            //HttpContext.Current.Response.Cookies.Add(httpCookie);
            var songUrl = "";
            if (item.SongId.IsNullorEmpty())
            {
                songUrl = item.SongUrl;
            }
            else
            {
                HttpClient client = new HttpClient();
                var result = await client.GetAsync(new Uri(Request.GetSongUrlById(item.SongId)));
                songUrl = result.Content.ToString().Replace("{\"url\":\"", "").Replace("\"}", "").Replace("\n", "").Replace("\\", "");
                if(songUrl.Contains("null"))
                { return null; }
            }

            var mediaSource = MediaSource.CreateFromUri(new Uri(songUrl));

            mediaSource.CustomProperties["SONG"] = item;
            var mediaPlaybackItem = new MediaPlaybackItem(mediaSource);
            var props = mediaPlaybackItem.GetDisplayProperties();

            await WritePropertiesAsync(item, props, item.AlbumImage);

            mediaPlaybackItem.ApplyDisplayProperties(props);
            return mediaPlaybackItem;
            //}
            //else
            //{
                //try
                //{
                //    /// **Local files can only create from <see cref="StorageFile"/>**
                //    var file = await StorageFile.GetFileFromPathAsync(item.FilePath);

                //    var img = await Core.Tools.Helper.UpdateSongAsync(item, file);

                //    var mediaSource = MediaSource.CreateFromStorageFile(file);

                //    mediaSource.CustomProperties[Consts.SONG] = item;
                //    var mediaPlaybackItem = new MediaPlaybackItem(mediaSource);
                //    var props = mediaPlaybackItem.GetDisplayProperties();

                //    if (img == null)
                //    {
                //        // Use black placeholder
                //        await WritePropertiesAsync(item, props, item.PicturePath);
                //    }
                //    else
                //    {
                //        // Use image stream
                //        WriteProperties(item, props, img);
                //    }
                //    mediaPlaybackItem.ApplyDisplayProperties(props);
                //    return mediaPlaybackItem;
                //}
                //catch (FileNotFoundException)
                //{
                //    item.IsEmpty = true;
                //    throw;
                //}
            //}
        }

        //private void WriteProperties(LuoVolSong item, MediaItemDisplayProperties props, RandomAccessStreamReference img)
        //{
        //    // When to Dispose img?
        //    props.Thumbnail = img;

        //    props.Type = Windows.Media.MediaPlaybackType.Music;
        //    props.MusicProperties.Title = item.Title.IsNullorEmpty() ? item.FilePath.Split('\\').LastOrDefault() : item.Title;
        //    props.MusicProperties.AlbumTitle = item.Album.IsNullorEmpty() ? "" : item.Album;
        //    props.MusicProperties.AlbumArtist = item.AlbumArtists.IsNullorEmpty() ? "" : string.Join(Consts.CommaSeparator, item.AlbumArtists);
        //    props.MusicProperties.AlbumTrackCount = item.TrackCount;
        //    props.MusicProperties.Artist = item.Performers.IsNullorEmpty() ? "" : string.Join(Consts.CommaSeparator, item.Performers);
        //    props.MusicProperties.TrackNumber = item.Track;
        //    if (!item.Genres.IsNullorEmpty())
        //    {
        //        foreach (var g in item.Genres)
        //        {
        //            props.MusicProperties.Genres.Add(g);
        //        }
        //    }
        //}

        private async Task WritePropertiesAsync(LuoVolSong item, MediaItemDisplayProperties props, string pic)
        {
            if (item.IsOnline)
            {
                if (pic.IsNullorEmpty())
                {
                    props.Thumbnail = null;
                }
                else
                {
                    props.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(pic));
                }
            }
            else
            {
                if (pic.IsNullorEmpty())
                {
                    props.Thumbnail = null;
                }
                else
                {
                    props.Thumbnail = RandomAccessStreamReference.CreateFromFile(await StorageFile.GetFileFromPathAsync(pic));
                }
            }

            props.Type = Windows.Media.MediaPlaybackType.Music;
            props.MusicProperties.Title = item.Name;
            props.MusicProperties.AlbumTitle = item.Album;
            props.MusicProperties.AlbumArtist = item.Artist;
            //props.MusicProperties.AlbumTrackCount = item.TrackCount;
            props.MusicProperties.Artist = item.Artist;
            //props.MusicProperties.TrackNumber = item.Track;
            //if (!item.Genres.IsNullorEmpty())
            //{
            //    foreach (var g in item.Genres)
            //    {
            //        props.MusicProperties.Genres.Add(g);
            //    }
            //}
        }

        public void PlayPause()
        {
            switch (MediaPlayer.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.Playing:
                    Pause();
                    break;
                case MediaPlaybackState.None:
                case MediaPlaybackState.Opening:
                case MediaPlaybackState.Buffering:
                case MediaPlaybackState.Paused:
                    Play();
                    break;
                default:
                    Play();
                    break;
            }
        }

        public void Loop(bool? b)
        {
            mediaPlaybackList.AutoRepeatEnabled = b ?? false;
        }

        public void LoopOne(bool? b)
        {
            MediaPlayer.IsLoopingEnabled = b ?? false;
        }

        public void Shuffle(bool? b)
        {
            mediaPlaybackList.ShuffleEnabled = b ?? false;
        }

        public void Next()
        {
            if (mediaPlaybackList.CurrentItem == null || mediaPlaybackList.Items.Count < 1)
            {
                return;
            }

            if (mediaPlaybackList.AutoRepeatEnabled)
            {
                mediaPlaybackList.MoveNext();

                //ChangeVolume(Settings.Current.PlayerVolume);
                return;
            }


            if (mediaPlaybackList.CurrentItemIndex == mediaPlaybackList.Items.Count - 1)
            {
                Stop();
                return;
            }

            mediaPlaybackList.MoveNext();
            //ChangeVolume(Settings.Current.PlayerVolume);
        }

        public void Stop()
        {
            if (mediaPlaybackList.Items.Count < 1)
            {
                return;
            }
            if (MediaPlayer.PlaybackSession.CanPause)
                MediaPlayer.Pause();
            MediaPlayer.Source = null;
            ItemsChanged?.Invoke(this, new PlayingItemsChangedArgs() { CurrentSong = null, CurrentIndex = -1, Items = null });
        }

        public void Seek(TimeSpan position)
        {
            if (MediaPlayer.PlaybackSession.CanSeek)
            {
                MediaPlayer.PlaybackSession.Position = position;
            }
        }

        public void Previous()
        {
            if (mediaPlaybackList.CurrentItem == null || mediaPlaybackList.Items.Count < 1)
            {
                return;
            }

            if (MediaPlayer.PlaybackSession.Position.TotalSeconds > 3)
            {
                MediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
                return;
            }

            if (mediaPlaybackList.AutoRepeatEnabled)
            {
                mediaPlaybackList.MovePrevious();
                //ChangeVolume(Settings.Current.PlayerVolume);
                return;
            }
            if (mediaPlaybackList.CurrentItemIndex == 0)
            {
                Stop();
                return;
            }
            mediaPlaybackList.MovePrevious();
            //ChangeVolume(Settings.Current.PlayerVolume);
        }

        public async void Play()
        {
            if (mediaPlaybackList == null || mediaPlaybackList.Items.IsNullorEmpty())
            {
                return;
            }

            if (MediaPlayer.Source == null)
            {
                MediaPlayer.Source = mediaPlaybackList;
            }

            int i = 0;
            while (mediaPlaybackList.CurrentItem == null)
            {
                //mediaPlaybackList.MoveNext();
                i++;
                if (i > 10)
                {
                    mediaPlaybackList.StartingItem = mediaPlaybackList.Items.First();
                    break;
                }
                await Task.Delay(200);
            }

            MediaPlayer.Play();
            await ToggleVolumeAnimation(true);
            //ChangeVolume(Settings.Current.PlayerVolume);
        }

        public async void Pause()
        {
            if (MediaPlayer.PlaybackSession.CanPause)
            {
                await ToggleVolumeAnimation(false);
                MediaPlayer.Pause();
            }              
        }


        public void Dispose()
        {
            MediaPlayer.Dispose();
        }

        public void SkiptoIndex(int index)
        {
            if (index >= 0 && index < mediaPlaybackList.Items.Count)
            {
                mediaPlaybackList.MoveTo((uint)index);
            }
        }

        public async Task ReloadCurrent()
        {
            var state = MediaPlayer.PlaybackSession.PlaybackState;
            var position = MediaPlayer.PlaybackSession.Position;

            var index = mediaPlaybackList.CurrentItemIndex;

            var cure = mediaPlaybackList.CurrentItem;
            mediaPlaybackList.Items.Remove(mediaPlaybackList.CurrentItem);
            cure.Source.Dispose();
            MediaPlayer.Source = null;

            if (index > currentList.Count)
            {

            }
            else
            {
                var item = currentList[(int)index];
                var mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);
                mediaPlaybackList.Items.Insert((int)index, mediaPlaybackItem);
                mediaPlaybackList.StartingItem = mediaPlaybackItem;
            }
            MediaPlayer.Source = mediaPlaybackList;
            MediaPlayer.PlaybackSession.Position = position;
            if (state == MediaPlaybackState.Playing)
            {
                MediaPlayer.Play();
            }
        }

        public void RemoveCurrentItem()
        {
            var state = MediaPlayer.PlaybackSession.PlaybackState;
            MediaPlayer.Pause();
            var cure = mediaPlaybackList.CurrentItem;
            mediaPlaybackList.MoveNext();
            cure.Source.Dispose();
            mediaPlaybackList.Items.Remove(cure);
            cure = null;
            if (state == MediaPlaybackState.Playing)
            {
                MediaPlayer.Play();
            }
        }

#pragma warning disable 1998
        public async Task DetachCurrentItem()
        {
            _savedState = MediaPlayer.PlaybackSession.PlaybackState;
            _savedPosition = MediaPlayer.PlaybackSession.Position;

            _savedIndex = mediaPlaybackList.CurrentItemIndex;

            var cure = mediaPlaybackList.CurrentItem;
            if (cure == null)
            {

            }
            else
            {
                mediaPlaybackList.Items.Remove(mediaPlaybackList.CurrentItem);
                cure.Source.Dispose();
            }
            MediaPlayer.Source = null;
        }

        public async Task ReAttachCurrentItem()
        {
            if (_savedIndex > currentList.Count)
            {

            }
            else
            {
                var item = currentList[(int)_savedIndex];
                var mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);
                mediaPlaybackList.Items.Insert((int)_savedIndex, mediaPlaybackItem);
                mediaPlaybackList.StartingItem = mediaPlaybackItem;
            }
            MediaPlayer.Source = mediaPlaybackList;
            MediaPlayer.PlaybackSession.Position = _savedPosition;
            if (_savedState == MediaPlaybackState.Playing)
            {
                MediaPlayer.Play();
            }
        }

        public async Task AddtoNextPlay(IList<LuoVolSong> items)
        {
            if (mediaPlaybackList.CurrentItem == null)
            {
                await NewPlayList(items);
                return;
            }

            var curIdx = (int)mediaPlaybackList.CurrentItemIndex;

            // NOTE: add to current song list
            currentList.InsertRange(curIdx + 1, items);

            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    var item = items[i];

                    var mediaPlaybackItem = await GetMediaPlaybackItemAsync(item);

                    if (newComing)
                        return;


                    mediaPlaybackList.Items.Insert(curIdx + 1 + i, mediaPlaybackItem);

                }
                catch (FileNotFoundException)
                {
                    continue;
                }
            }
            ItemsChanged?.Invoke(this, new PlayingItemsChangedArgs()
            {
                IsShuffle = isShuffle,
                IsOneLoop = MediaPlayer.IsLoopingEnabled,
                IsLoop = mediaPlaybackList.AutoRepeatEnabled,
                CurrentSong = currentList[curIdx],
                CurrentIndex = curIdx,
                Items = currentList
            });
        }       

        public void Backward(TimeSpan timeSpan)
        {
            var p = MediaPlayer.PlaybackSession.Position - timeSpan;
            if (p.TotalMilliseconds < 0)
            {
                MediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
                Previous();
            }
            else
            {
                MediaPlayer.PlaybackSession.Position -= timeSpan;
            }
        }

        public void Forward(TimeSpan timeSpan)
        {
            var p = MediaPlayer.PlaybackSession.Position + timeSpan;
            if (p > MediaPlayer.PlaybackSession.NaturalDuration)
            {
                Next();
            }
            else
            {
                MediaPlayer.PlaybackSession.Position += timeSpan;
            }
        }

        public void RefreshNowPlayingInfo()
        {
            PlaybackStatusChanged?.Invoke(this, new PlaybackStatusChangedArgs()
            {
                PlaybackStatus = MediaPlayer.PlaybackSession.PlaybackState,
                IsOneLoop = MediaPlayer.IsLoopingEnabled,
                IsLoop = mediaPlaybackList.AutoRepeatEnabled,
                IsShuffle = isShuffle
            });
            if (mediaPlaybackList.CurrentItem != null)
            {
                var currentSong = mediaPlaybackList.CurrentItem.Source.CustomProperties["SONG"] as LuoVolSong;
                ItemsChanged?.Invoke(this, new PlayingItemsChangedArgs
                {
                    IsShuffle = isShuffle,
                    IsOneLoop = MediaPlayer.IsLoopingEnabled,
                    IsLoop = mediaPlaybackList.AutoRepeatEnabled,
                    CurrentSong = currentSong,
                    CurrentIndex = currentSong == null ? -1 : currentList.FindIndex(a => a == currentSong),
                    Items = currentList,
                    Thumnail = mediaPlaybackList.CurrentItem?.GetDisplayProperties().Thumbnail
                });
            }
            else
            {
                ItemsChanged?.Invoke(this, new PlayingItemsChangedArgs() { CurrentSong = null, CurrentIndex = -1, Items = null });
            }
        }
    }
}
