using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using LuoMusic.ViewModel;
using Luo.Shared.Data;
using System;
using LuoMusic.Model;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;
using LuoMusic.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Luo.Shared.Convertor;
using LuoMusic.Common;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Luo.Shared.Helper;

namespace LuoMusic
{
    public sealed partial class MainPage
    {
        public static MainPage Current;

        public MainViewModel MainVM => (MainViewModel)DataContext;

        public MainPage()
        {
            InitializeComponent();

            Current = this;

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

            MainSlider.AddHandler(PointerReleasedEvent, new PointerEventHandler(MainSlider_PointerReleased), true);

            SetSliderBinding();

            MainVM.AboutToUpdateSelectedNavIndex += MainVM_AboutToUpdateSelectedNavIndex;

            MainFrame.Navigate((MainVM.HamList[0] as HamPanelItem).TargetType);

            SystemNavigationManager.GetForCurrentView().BackRequested += OnMainPageBackRequested;

            //LuoVolFactory.getlist();
        }

        public async void NavigateByVolNum(string volnum)
        {
            var vol = new LuoVol() {
                VolNum = volnum,
                VolUrl = Request.GetNumVol_q(volnum)
            };
            MainVM.CurrentVol = new VolItem(vol);
            await MainVM.CurrentVol.GetVolDetialAsync();
            MainFrame.Navigate(typeof(VolDetialPage));
        }

        private void OnMainPageBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainFrame != null && MainFrame.CanGoBack)
            {
                e.Handled = true;
                MainFrame.GoBack();
                UpdateNavIndex();
            }
        }

        private void UpdateNavIndex()
        {
            if(MainFrame.Content.GetType() != typeof(VolDetialPage) && MainFrame.Content.GetType() != typeof(VolPlayDetialPage))
            {
                if ((MainVM.CurrentNavIndex == -1) || (MainVM.HamList[MainVM.CurrentNavIndex].TargetType != MainFrame.Content.GetType()))
                {
                    var currentNavIndex = MainVM.HamList.IndexOf(MainVM.HamList.Find(p => p.TargetType == MainFrame.Content.GetType()));
                    MainVM.CurrentNavIndex = currentNavIndex;
                }
            }
            else
            { return; }
        }

        private void MainVM_AboutToUpdateSelectedNavIndex(object sender, int e)
        {
            var index = e;
            if (index < 0)
                return;
            if(MainVM.HamList[MainVM.CurrentNavIndex].TargetType != MainFrame.Content.GetType())
            {
                MainFrame.Navigate((MainVM.HamList[index] as HamPanelItem).TargetType);
            }
            else
            { return; }
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
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VolListPage.MainNavigateToEvent += VolListPage_MainNavigateToEvent;
            VolTagListPage.MainNavigateToEvent += VolTagListPage_MainNavigateToEvent;
            HeartPage.MainNavigateToEvent += HeartPage_MainNavigateToEvent;
        }

        private void HeartPage_MainNavigateToEvent(Type page)
        {
            MainFrame.Navigate(page);
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

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VolItem item = (VolItem)e.ClickedItem;
            await item.GetVolDetialAsync();
            MainVM.CurrentVol = item;
        }

        private async void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            LuoVolSong item = (LuoVolSong)e.ClickedItem;
            //MainVM.CurrentSong = item;
            //Player.Source=MediaSource.CreateFromUri(new Uri(item.SongUrl));
            await MainVM.InstantPlayAsync(MainVM.CurrentVol.Vol.VolSongs, MainVM.CurrentVol.Vol.VolSongs.IndexOf(item));
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if(MainFrame.Content.GetType()==typeof(VolDetialPage))
            {
                MainVM.NeedShowBack = true;
            }
            else 
            {
                MainVM.NeedShowBack = false;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
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
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                Window.Current.SetTitleBar(TitleBar);
            }
            //if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            //{
            //    TitleBar.Visibility = Visibility.Collapsed;
            //}
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

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GoBack();
        }

        private void MainNavigation_TitleClicked(object sender, TitleClickEventArg e)
        {
            if (e.NewIndex == e.OldIndex)
            {
                if (MainFrame.Content.GetType() == typeof(VolListPage))
                {
                    VolListPage._volListPage.ScrollToTop();
                }
                if (MainFrame.Content.GetType() == typeof(VolTagListPage))
                {
                    VolTagListPage._volTagListPage.ScrollToTop();
                }
                if (MainFrame.Content.GetType() == typeof(VolDetialPage) || MainFrame.Content.GetType() == typeof(VolPlayDetialPage))
                {
                    GoBack();
                }
            }
        }

        private void RelateVol_Click(object sender, RoutedEventArgs e)
        {
            if(MainVM.CurrentPlayVol != null)
            {
                if (MainFrame.Content.GetType() != typeof(VolPlayDetialPage))
                {
                    if((MainVM.CurrentPlayVol == MainVM.CurrentVol)&&(MainFrame.Content.GetType() == typeof(VolDetialPage)))
                    {
                        return;
                    }
                    else
                    {
                        MainFrame.Navigate(typeof(VolPlayDetialPage));
                    }
                }
                else
                {
                    GoBack();
                }
            }
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            if(MainFrame.Content.GetType() == typeof(SettingPage))
            {
                GoBack();
                UpdateNavIndex();
            }
            else
            {
                MainFrame.Navigate(typeof(SettingPage));
                MainVM.CurrentNavIndex = -1;
            }
        }

        private void MoreBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content.GetType() == typeof(AboutPage))
            {
                GoBack();
                UpdateNavIndex();
            }
            else
            {
                MainFrame.Navigate(typeof(AboutPage));
                MainVM.CurrentNavIndex = -1;
            }
        }

        private void Artwork_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(MainVM.CurrentSong != null)
            {
                MainVM.PresentSongPlayCommand.Execute(null);
            }
        }

        private void SongPlayUC_OnPresentedChanged(object sender, PresentedArgs e)
        {
            if(!MainVM.SongPlayPagePresented && MainVM.CurrentvolDetailPagePresented)
            {
                if (MainFrame.Content.GetType() != typeof(VolPlayDetialPage))
                {
                    if ((MainVM.CurrentPlayVol == MainVM.CurrentVol) && (MainFrame.Content.GetType() == typeof(VolDetialPage)))
                    {
                        return;
                    }
                    else
                    {
                        MainFrame.Navigate(typeof(VolPlayDetialPage));
                    }
                }
                MainVM.CurrentvolDetailPagePresented = false;
            }
        }

        private void SongPlayUC_OnPresentedChanged(object sender, object e)
        {

        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (MainVM.CurrentSong != null)
            {
                ArtworkBorder.Visibility = Visibility.Visible;
            }
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (MainVM.CurrentSong != null)
            {
                ArtworkBorder.Visibility = Visibility.Collapsed;
            }
        }
    }
}
