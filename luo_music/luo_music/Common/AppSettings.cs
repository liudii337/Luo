using GalaSoft.MvvmLight;
using Luo.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace LuoMusic.Common
{
    public class AppSettings : ViewModelBase
    {
        private const int LightTheme = 0;
        private const int DarkTheme = 1;
        private const int SystemTheme = 2;

        public ApplicationDataContainer LocalSettings { get; set; }

        public int Volume
        {
            get
            {
                return ReadSettings(nameof(Volume), 100);
            }
            set
            {
                SaveSettings(nameof(Volume), value);
                RaisePropertyChanged(() => Volume);
            }
        }


        //public bool EnableTile
        //{
        //    get
        //    {
        //        return ReadSettings(nameof(EnableTile), true);
        //    }
        //    set
        //    {
        //        SaveSettings(nameof(EnableTile), value);
        //        RaisePropertyChanged(() => EnableTile);

        //        //Events.LogTile(value);

        //        //if (!value)
        //        //{
        //        //    LiveTileUpdater.CleanUpTile();
        //        //}
        //    }
        //}

        //public bool EnableScaleAnimation
        //{
        //    get
        //    {
        //        return ReadSettings(nameof(EnableScaleAnimation), true);
        //    }
        //    set
        //    {
        //        SaveSettings(nameof(EnableScaleAnimation), value);
        //        RaisePropertyChanged(() => EnableScaleAnimation);

        //        //Events.LogScaleAnimation(value);
        //    }
        //}

        //public string SaveFolderPath
        //{
        //    get
        //    {
        //        return ReadSettings(nameof(SaveFolderPath), "");
        //    }
        //    set
        //    {
        //        SaveSettings(nameof(SaveFolderPath), value);
        //        RaisePropertyChanged(() => SaveFolderPath);
        //    }
        //}

        //public int DefaultCategory
        //{
        //    get
        //    {
        //        return ReadSettings(nameof(DefaultCategory), 0);
        //    }
        //    set
        //    {
        //        SaveSettings(nameof(DefaultCategory), value);
        //        RaisePropertyChanged(() => DefaultCategory);
        //    }
        //}

        //public int SaveQuality
        //{
        //    get
        //    {
        //        return ReadSettings(nameof(SaveQuality), 1);
        //    }
        //    set
        //    {
        //        SaveSettings(nameof(SaveQuality), value);
        //        RaisePropertyChanged(() => SaveQuality);
        //    }
        //}

        //public int Language
        //{
        //    get
        //    {
        //        return ReadSettings(nameof(Language), 0);
        //    }
        //    set
        //    {
        //        SaveSettings(nameof(Language), value);
        //        RaisePropertyChanged(() => Language);
        //        //ToastService.SendToast(ResourcesHelper.GetResString("RestartHint"), 3000);

        //        //Events.LogSwitchLanguage(value);
        //    }
        //}


        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        /// <summary>
        /// Invoked on User change theme in Windows' Settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>

        public async Task<StorageFolder> GetSavingFolderAsync()
        {
            var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerSplash", CreationCollisionOption.OpenIfExists);
            return folder;
        }

        public async Task<StorageFolder> GetWallpaperFolderAsync()
        {
            var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("WallpapersTemp", CreationCollisionOption.OpenIfExists);
            return folder;
        }

        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        private T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            if (defaultValue != null)
            {
                return defaultValue;
            }
            return default(T);
        }

        private static readonly Lazy<AppSettings> lazy = new Lazy<AppSettings>(() => new AppSettings());

        public static AppSettings Instance { get { return lazy.Value; } }
    }

}
