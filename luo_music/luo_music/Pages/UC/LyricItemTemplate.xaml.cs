using LuoMusic.ViewModel.DataViewModel;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace LuoMusic.Pages.UC
{
    public sealed partial class LyricItemTemplate : UserControl
    {
        public LyricItemTemplate()
        {
            this.InitializeComponent();
        }

        public LrcContent Data
        {
            get { return (LrcContent)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(LrcContent), typeof(LyricItemTemplate), new PropertyMetadata(null));

    }

    public class LyricTrigger : StateTriggerBase
    {


        public bool IsCurrent
        {
            get { return (bool)GetValue(IsCurrentProperty); }
            set { SetValue(IsCurrentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCurrent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCurrentProperty =
            DependencyProperty.Register("IsCurrent", typeof(bool), typeof(LyricTrigger), new PropertyMetadata(false, OnIsCurrentChanged));

        private static void OnIsCurrentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LyricTrigger l)
            {
                l.SetActive((bool)e.NewValue);
            }
        }
    }
}

