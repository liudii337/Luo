﻿using LuoMusic.Model;
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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace LuoMusic.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HeartPage : Page
    {
        public MainViewModel MainVM => (MainViewModel)DataContext;

        public delegate void NavigateHandel(Type page);
        public static event NavigateHandel MainNavigateToEvent;

        public HeartPage()
        {
            this.InitializeComponent();
        }

        private void VolListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VolItem item = (VolItem)e.ClickedItem;

            if (!item.Vol.IsDetailGet)
            {
                item.GetVolDetialAsync();
            }

            MainVM.CurrentVol = item;
            MainNavigateToEvent(typeof(VolDetialPage));
        }
    }
}
