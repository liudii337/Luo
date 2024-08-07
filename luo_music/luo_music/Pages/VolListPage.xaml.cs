﻿using JP.Utils.UI;
using LuoMusic.Common;
using LuoMusic.Common.Composition;
using LuoMusic.Model;
using LuoMusic.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
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
    public sealed partial class VolListPage : Page
    {
        public MainViewModel MainVM => (MainViewModel)DataContext;

        public static VolListPage _volListPage;

        public VolListPage()
        {
            this.InitializeComponent();
            _volListPage = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        public delegate void NavigateHandel(Type page);
        public static event NavigateHandel MainNavigateToEvent;

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        { 
            VolItem item = (VolItem)e.ClickedItem;

            if(!item.Vol.IsDetailGet)
            {
                item.GetVolDetialAsync();
            }

            MainVM.CurrentVol = item;
            
            var container = (GridViewItem)VolListGridView.ContainerFromItem(item);
            var root = (FrameworkElement)container.ContentTemplateRoot;
            var CoverImage = (Image)root.FindName("Cover");

            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("forwardAnimation", CoverImage);

            MainNavigateToEvent(typeof(VolDetialPage));

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(MainVM.CurrentVol!=null)
            {
                var container = (GridViewItem)VolListGridView.ContainerFromItem(MainVM.CurrentVol);
                if (container != null)
                {
                    var root = (FrameworkElement)container.ContentTemplateRoot;
                    var CoverImage = (Image)root.FindName("Cover");

                    ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("backwardAnimation");
                    if (animation != null)
                    {
                        animation.TryStart(CoverImage);
                    }
                }
            }
        }

        public void ScrollToTop()
        {
            VolListGridView.GetScrollViewer().ChangeView(null, 0, null);
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if(MainVM.CurrentNumIndex < MainVM.LuoVolNums.Count-1)
            {
                MainVM.CurrentNumIndex++;
            }
        }

        private void LastPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainVM.CurrentNumIndex > 0)
            {
                MainVM.CurrentNumIndex--;
            }
        }

        private void NumGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NextPageButton.Visibility = (MainVM.CurrentNumIndex < MainVM.LuoVolNums.Count - 1) == true ? Visibility.Visible : Visibility.Collapsed;
            LastPageButton.Visibility = (MainVM.CurrentNumIndex > 0) == true ? Visibility.Visible : Visibility.Collapsed;

        }
    }
}
