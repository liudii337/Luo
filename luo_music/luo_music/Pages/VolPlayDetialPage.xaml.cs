using Luo.Shared.Data;
using LuoMusic.ViewModel;
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

namespace LuoMusic.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VolPlayDetialPage : Page
    {
        public MainViewModel MainVM => (MainViewModel)DataContext;

        public VolPlayDetialPage()
        {
            this.InitializeComponent();
        }

        private async void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            LuoVolSong item = (LuoVolSong)e.ClickedItem;
            //MainVM.CurrentSong = item;
            //Player.Source=MediaSource.CreateFromUri(new Uri(item.SongUrl));
            if ((MainVM.CurrentPlayVol == MainVM.CurrentVol) && (MainVM.CurrentSong == item))
            {
                if (MainVM.IsPlaying)
                {
                    MainVM.player.Pause();
                }
                else
                {
                    MainVM.player.Play();
                }
            }
            else
            {
                MainVM.CurrentPlayVol = MainVM.CurrentVol;
                await MainVM.InstantPlayAsync(MainVM.CurrentVol.Vol.VolSongs, MainVM.CurrentVol.Vol.VolSongs.IndexOf(item));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(MainVM.CurrentSong!=null)
            {
                if(MainVM.CurrentSong.VolNum !=MainVM.CurrentVol.Vol.VolNum)
                {
                    ((ListView)sender).SelectedIndex = -1;
                    return;
                }
            }

            //var ab = (LuoVolSong)e.AddedItems[0];
            //var _item = SongListView.ContainerFromItem(ab) as ListViewItem;
            ////var test= _item.ContentTemplate.GetElement()
            //Storyboard sb = _item.Resources["test"] as Storyboard;
            //sb.Begin();
            //if (e.RemovedItems.Count()!=0)
            //{
            //    var cd = (LuoVolSong)e.RemovedItems[0];

            //}

            
        }
    }
}
