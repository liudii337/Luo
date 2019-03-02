using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using luo_music.ViewModel;
using Luo.Shared.Data;
using System;
using luo_music.Model;

namespace luo_music
{
    public sealed partial class MainPage
    {
        public MainViewModel MainVm => (MainViewModel)DataContext;

        public MainPage()
        {
            InitializeComponent();

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

            Loaded += (s, e) =>
            {
                MainVm.RunClock();
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
            MainVm.StopClock();
            base.OnNavigatingFrom(e);
        }

        private async void ListView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            VolItem item = (VolItem)e.ClickedItem;
            await item.GetVolDetialAsync();
            MainVm.CurrentVol = item;
            MainVm.StopClock();
        }
    }
}
