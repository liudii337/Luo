﻿using Luo.Shared.Data;
using luo_music.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace luo_music.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VolDetialPage : Page
    {
        public MainViewModel MainVM => (MainViewModel)DataContext;

        public VolDetialPage()
        {
            this.InitializeComponent();
        }

        private async void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            LuoVolSong item = (LuoVolSong)e.ClickedItem;
            //MainVM.CurrentSong = item;
            //Player.Source=MediaSource.CreateFromUri(new Uri(item.SongUrl));
            await MainVM.InstantPlayAsync(MainVM.CurrentVol.Vol.VolSongs, MainVM.CurrentVol.Vol.VolSongs.IndexOf(item));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainVM.NeedShowBack = true;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(MainVM.CurrentSong!=null)
            {
                if(MainVM.CurrentSong.VolNum !=MainVM.CurrentVol.Vol.VolNum)
                {
                    ((ListView)sender).SelectedIndex = -1;
                }
            }

            
            var ab = (LuoVolSong)e.AddedItems[0];
            var _item = SongListView.ContainerFromItem(ab) as ListViewItem;
            //var test= _item.ContentTemplate.GetElement()
            Storyboard sb = _item.Resources["test"] as Storyboard;
            sb.Begin();
            if (e.RemovedItems.Count()!=0)
            {
                var cd = (LuoVolSong)e.RemovedItems[0];

            }

            
        }
    }
}
