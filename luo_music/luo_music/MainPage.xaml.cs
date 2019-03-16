﻿using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using luo_music.ViewModel;
using Luo.Shared.Data;
using System;
using luo_music.Model;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;
using luo_music.Pages;
using Windows.UI.Xaml;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VolListPage.MainNavigateToEvent += VolListPage_MainNavigateToEvent;
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
    }
}
