using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Debug;
using Luo.Shared.Data;
using LuoMusic.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace LuoMusic.ViewModel.DataViewModel
{
    public class HeartViewModel : ViewModelBase
    {
        public static string CACHED_Vol_FILE_NAME => "DownloadList.vollist";
        public static string CACHED_Song_FILE_NAME => "DownloadList.songlist";

        private readonly object o = new object();

        private ObservableCollection<VolItem> _heartVols;
        public ObservableCollection<VolItem> HeartVols
        {
            get
            {
                return _heartVols;
            }
            set
            {
                if (_heartVols != value)
                {
                    _heartVols = value;
                    RaisePropertyChanged(() => HeartVols);
                    _heartVols.CollectionChanged += HeartVols_CollectionChanged; ;
                    NoVolItemVisibility = value.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private ObservableCollection<LuoVolSong> _heartSongs;
        public ObservableCollection<LuoVolSong> HeartSongs
        {
            get
            {
                return _heartSongs;
            }
            set
            {
                if (_heartSongs != value)
                {
                    _heartSongs = value;
                    RaisePropertyChanged(() => HeartSongs);
                    _heartSongs.CollectionChanged += HeartSongs_CollectionChanged; ;
                    NoSongItemVisibility = value.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private Visibility _noVolItemVisibility;
        public Visibility NoVolItemVisibility
        {
            get
            {
                return _noVolItemVisibility;
            }
            set
            {
                if (_noVolItemVisibility != value)
                {
                    _noVolItemVisibility = value;
                    RaisePropertyChanged(() => NoVolItemVisibility);
                }
            }
        }

        private Visibility _noSongItemVisibility;
        public Visibility NoSongItemVisibility
        {
            get
            {
                return _noSongItemVisibility;
            }
            set
            {
                if (_noSongItemVisibility != value)
                {
                    _noSongItemVisibility = value;
                    RaisePropertyChanged(() => NoSongItemVisibility);
                }
            }
        }


        public HeartViewModel()
        {
            HeartSongs = new ObservableCollection<LuoVolSong>();
            HeartVols = new ObservableCollection<VolItem>();
            //var task = RestoreListAsync();
        }

        private void HeartVols_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoVolItemVisibility = HeartVols.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    //await(item as DownloadItem).AwaitGuidCreatedAsync();
                }
            }
            lock (o)
            {
                var task = SaveVolListAsync();
            }
        }

        private void HeartSongs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoSongItemVisibility = HeartVols.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    //await(item as DownloadItem).AwaitGuidCreatedAsync();
                }
            }
            lock (o)
            {
                var task = SaveSongListAsync();
            }
        }


        public async Task SaveVolListAsync()
        {
            var str = JsonConvert.SerializeObject(HeartVols, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Error = async (s, e) =>
                {
                    await Logger.LogAsync(e.ErrorContext.Error);
                }
            });
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CACHED_Vol_FILE_NAME,
                CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, str);
        }

        public async Task SaveSongListAsync()
        {
            var str = JsonConvert.SerializeObject(HeartSongs, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Error = async (s, e) =>
                {
                    await Logger.LogAsync(e.ErrorContext.Error);
                }
            });
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CACHED_Song_FILE_NAME,
                CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, str);
        }

#pragma warning disable

        public async Task RestoreListAsync()
        {
            //Restore Vol
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CACHED_Vol_FILE_NAME,
                CreationCollisionOption.OpenIfExists);
            if (file != null)
            {
                var str = await FileIO.ReadTextAsync(file);
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        var list = JsonConvert.DeserializeObject<ObservableCollection<VolItem>>(str, new JsonSerializerSettings()
                        {
                            Error = (s, e) =>
                            {
                                var msg = e.ErrorContext.Error.Message;
                            },
                            TypeNameHandling = TypeNameHandling.All
                        });
                        if (list != null)
                        {
                            HeartVols = list;
                            foreach (var item in list)
                            {
                                //item.IsHeartVol=true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        var a=e.Message;
                    }
                }
            }

            //Restore Song
            file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CACHED_Song_FILE_NAME,
                CreationCollisionOption.OpenIfExists);
            if (file != null)
            {
                var str = await FileIO.ReadTextAsync(file);
                if (!string.IsNullOrEmpty(str))
                {
                    var list = JsonConvert.DeserializeObject<ObservableCollection<LuoVolSong>>(str, new JsonSerializerSettings()
                    {
                        Error = (s, e) =>
                        {
                            var msg = e.ErrorContext.Error.Message;
                        },
                        TypeNameHandling = TypeNameHandling.All
                    });
                    if (list != null)
                    {
                        HeartSongs = list;
                        foreach (var item in list)
                        {
                            //HeartSongs.Add(item);
                        }
                    }
                }
            }
        }

#pragma warning restore
        public bool IsHeartedVol(VolItem item)
        {
            return HeartVols.Any(p => p.TitleString == item.TitleString);
        }
        public bool IsHeartedVol(LuoVol item)
        {
            return HeartVols.Any(p => p.Vol.Title == item.Title);
        }

        public bool IsHeartedSong(LuoVolSong item)
        {
            return HeartSongs.Any(p => p.Name == item.Name);
        }

        public void AddVol(VolItem item)
        {
            if (!IsHeartedVol(item))
            { HeartVols.Add(item); }
        }
        public void RemoveVol(VolItem item)
        {
            HeartVols.Remove(item);
        }

        public void AddSong(LuoVolSong item)
        {
            if (!IsHeartedSong(item))
            { HeartSongs.Add(item); }
        }
        public void RemoveSong(LuoVolSong item)
        {
            HeartSongs.Remove(item);
        }

        private RelayCommand _clearVolCommand;
        public RelayCommand ClearVolCommand
        {
            get
            {
                if (_clearVolCommand != null) return _clearVolCommand;
                return _clearVolCommand = new RelayCommand(async () =>
                {
                    HeartVols?.Clear();
                    await SaveVolListAsync();
                });
            }
        }

        private RelayCommand _clearSongCommand;
        public RelayCommand ClearSongCommand
        {
            get
            {
                if (_clearSongCommand != null) return _clearSongCommand;
                return _clearSongCommand = new RelayCommand(async () =>
                {
                    HeartSongs?.Clear();
                    await SaveSongListAsync();
                });
            }
        }

    }

}
