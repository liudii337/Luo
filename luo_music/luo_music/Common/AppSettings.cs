using GalaSoft.MvvmLight;
using Luo.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;

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

        public bool EnableTile
        {
            get
            {
                return ReadSettings(nameof(EnableTile), true);
            }
            set
            {
                SaveSettings(nameof(EnableTile), value);
                RaisePropertyChanged(() => EnableTile);

                //Events.LogTile(value);
            }
        }

        public bool EnableTimeline
        {
            get
            {
                return ReadSettings(nameof(EnableTimeline), true);
            }
            set
            {
                SaveSettings(nameof(EnableTimeline), value);
                RaisePropertyChanged(() => EnableTimeline);
            }
        }

        public bool EnableCheckLatestVol
        {
            get
            {
                return ReadSettings(nameof(EnableCheckLatestVol), true);
            }
            set
            {
                SaveSettings(nameof(EnableCheckLatestVol), value);
                RaisePropertyChanged(() => EnableCheckLatestVol);

                if(EnableCheckLatestVol)
                { var task1 = BackgroundTaskRegister.RegisterAsync(); }
                else
                { var task1 = BackgroundTaskRegister.UnregisterAsync(); }
            }
        }

        public string LatestVolNum
        {
            get
            {
                return ReadSettings(nameof(LatestVolNum), "1038");
            }
            set
            {
                SaveSettings(nameof(LatestVolNum), value);
                RaisePropertyChanged(() => LatestVolNum);
            }
        }

        private bool _isLight;
        public bool IsLight
        {
            get
            {
                return _isLight;
            }
            set
            {
                _isLight = value;
                //
                Tools.SetStatusBar(IsLight);
            }
        }

        public int ThemeMode
        {
            get
            {
                return ReadSettings(nameof(ThemeMode), SystemTheme);
            }
            set
            {
                SaveSettings(nameof(ThemeMode), value);
                RaisePropertyChanged(() => ThemeMode);

                ElementTheme theme;
                switch (value)
                {
                    case LightTheme:
                        theme = ElementTheme.Light;
                        IsLight = true;
                        break;
                    case DarkTheme:
                        theme = ElementTheme.Dark;
                        IsLight = false;
                        break;
                    default:
                        theme = ElementTheme.Default;
                        break;
                }
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = theme;

                    // If the user switch to follow the system, then we apply the App's theme instead of element's theme.
                    if (theme == ElementTheme.Default)
                    {
                        IsLight = Application.Current.RequestedTheme == ApplicationTheme.Light;
                    }
                }

            }
        }


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

        public int Language
        {
            get
            {
                return ReadSettings(nameof(Language), 0);
            }
            set
            {
                SaveSettings(nameof(Language), value);
                RaisePropertyChanged(() => Language);
                ApplicationLanguages.PrimaryLanguageOverride = value == 1 ? "zh-CN" : "en-US";
                ToastService.SendToast(ResourcesHelper.GetResString("RestartHint"), 3000);

                //Events.LogSwitchLanguage(value);
            }
        }

        private readonly UISettings _uiSettings;

        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
            ThemeMode = ThemeMode;

            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += Settings_ColorValuesChanged;

            EnableCheckLatestVol = EnableCheckLatestVol;
            LatestVolNum = LatestVolNum;

            var language = ApplicationLanguages.PrimaryLanguageOverride;
            if (language == "")
            {
                var languages = ApplicationLanguages.Languages;
                if (languages.Count > 0)
                {
                    var primary = languages[0];
                    SaveSettings(nameof(Language), primary?.Contains("zh") ?? false ? 1 : 0);
                }
            }

        }

        private async void Settings_ColorValuesChanged(UISettings sender, object args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateThemeToSystemTheme();
            });
        }

        private void UpdateThemeToSystemTheme()
        {
            if (ThemeMode == SystemTheme)
            {
                // Currently the theme of Application should be the same as System's.
                IsLight = Application.Current.RequestedTheme == ApplicationTheme.Light;
            }
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
