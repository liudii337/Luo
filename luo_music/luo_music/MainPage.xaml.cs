using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using luo_music.ViewModel;
using Luo.Shared.Data;
using System;
using luo_music.Model;
using Windows.Media.Core;

namespace luo_music
{
    public sealed partial class MainPage
    {
        public MainViewModel MainVM => (MainViewModel)DataContext;

        public MainPage()
        {
            InitializeComponent();

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

            Loaded += (s, e) =>
            {
                MainVM.RunClock();
            };

            //LuoVolFactory.getlist();
        }

        private void SystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            MainVM.StopClock();
            base.OnNavigatingFrom(e);
        }

        private async void ListView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            VolItem item = (VolItem)e.ClickedItem;
            await item.GetVolDetialAsync();
            MainVM.CurrentVol = item;
            MainVM.StopClock();
        }

        private async void ListView_ItemClick_1(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            LuoVolSong item = (LuoVolSong)e.ClickedItem;
            //MainVM.CurrentSong = item;
            //Player.Source=MediaSource.CreateFromUri(new Uri(item.SongUrl));
            await MainVM.InstantPlayAsync(MainVM.CurrentVol.Vol.VolSongs, MainVM.CurrentVol.Vol.VolSongs.IndexOf(item));
        }
    }
}
