using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using luo_music.ViewModel;
using Luo.Shared.Data;
using System;
using luo_music.Model;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;
using luo_music.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Luo.Shared.Convertor;

namespace luo_music
{
    public sealed partial class MainPage
    {
        public MainViewModel MainVM => (MainViewModel)DataContext;

        public MainPage()
        {
            InitializeComponent();

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

            MainSlider.AddHandler(PointerReleasedEvent, new PointerEventHandler(MainSlider_PointerReleased), true);

            SetSliderBinding();

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VolListPage.MainNavigateToEvent += VolListPage_MainNavigateToEvent;
            VolTagListPage.MainNavigateToEvent += VolTagListPage_MainNavigateToEvent;
            CategoryPage.MainNavigateToEvent += CategoryPage_MainNavigateToEvent;
        }

        private void VolTagListPage_MainNavigateToEvent(Type page)
        {
            MainFrame.Navigate(page);
        }

        private void CategoryPage_MainNavigateToEvent(Type page)
        {
            MainFrame.Navigate(page);
        }

        private void VolListPage_MainNavigateToEvent(Type page)
        {
            MainFrame.Navigate(page);
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

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            HamPane.SelectionChanged -= HamPane_SelectionChanged;
            if(MainFrame.Content.GetType()==typeof(VolDetialPage))
            {
                MainVM.NeedShowBack = true;
            }
            else if(MainFrame.Content.GetType() == typeof(VolTagListPage))
            {
                MainVM.NeedShowBack = true;
            }
            else
            {
                var index = MainVM.HamList.FindIndex(a => a.TargetType == MainFrame.Content.GetType());
                HamPane.SelectedIndex = index;
            }
            HamPane.SelectionChanged += HamPane_SelectionChanged;
        }

        private void HamPane_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as ListView;
            var index = HamPane.SelectedIndex;
            if (index < 0)
                return;

            MainFrame.Navigate((MainVM.HamList[index] as HamPanelItem).TargetType);
        }

        private void BackButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GoBack();
        }

        internal bool GoBack()
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
                return true;
            }
            else
            {
                MainVM.NeedShowBack = false;
                return false;
            }
        }

        private void TitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SetTitleBar(TitleBar);
        }

        private void HamPane_Loaded(object sender, RoutedEventArgs e)
        {
            HamPane.SelectedIndex = 0;
        }

        private void HamPane_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        public string PositionToString(TimeSpan t1, TimeSpan total)
        {
            if (total == null || total == default(TimeSpan))
            {
                return "0:00/0:00";
            }
            return $"{$"{(int)(Math.Floor(t1.TotalMinutes))}:{t1.Seconds.ToString("00")}"}/{$"{(int)(Math.Floor(total.TotalMinutes))}:{total.Seconds.ToString("00")}"}";
        }

        private void MainSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            MainVM.ChangePlayPosition(TimeSpan.FromSeconds(MainSlider.Value));
            SetSliderBinding();
        }

        private void SetSliderBinding()
        {
            var convertor = new DoubleTimespanConvertor();
            MainSlider.SetBinding(Slider.ValueProperty, new Binding() { Source = MainVM, Path = new PropertyPath("CurrentPosition"), Converter = convertor, Mode = BindingMode.OneWay });
        }

        private void MainSlider_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainVM.ChangePlayPosition(TimeSpan.FromSeconds(MainSlider.Value));
        }
    }
}
