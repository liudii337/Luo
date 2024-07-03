using Luo.Shared.Data;
using Luo.Shared.Helper;
using LuoMusic.Common;
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
    public sealed partial class SongPlayPage : NavigableUserControl
    {
        private Storyboard _rotateStoryboard;

        public MainViewModel MainVM => (MainViewModel)DataContext;
        
        public SongPlayPage()
        {
            this.InitializeComponent();

            _rotateStoryboard = new Storyboard();
            DoubleAnimation rotateAnimation = new DoubleAnimation()
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(4)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(rotateAnimation, ImageCompositeTransform);
            Storyboard.SetTargetProperty(rotateAnimation, "Rotation");
            _rotateStoryboard.Children.Add(rotateAnimation);

            MainVM.PropertyChanged += MainVM_PropertyChanged; ;
        }

        private void MainVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainVM.IsPlaying))
            {
                if (MainVM.IsPlaying)
                {
                    _rotateStoryboard.Begin();
                }
                else
                {
                    _rotateStoryboard.Pause();
                }
            }
        }

        private async void LrcView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainVM.LyricVM.Contents.Count > 0 && (sender as ListView).SelectedIndex >= 0)
                try
                {
                    await(sender as ListView).ScrollToIndex((sender as ListView).SelectedIndex, ScrollPosition.Center);
                }
                catch (Exception)
                {
                }
        }

        private void DropShadowPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(this.ActualWidth<850)
            {
                SongPanel.Visibility = Visibility.Collapsed;
                LrcGrid.Visibility = Visibility.Visible;
            }
        }

        private void LrcGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.ActualWidth < 850)
            {
                SongPanel.Visibility = Visibility.Visible;
                LrcGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void Vol_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainVM.CurrentvolDetailPagePresented = true;
            MainVM.SongPlayPagePresented = false;
        }
    }
}
