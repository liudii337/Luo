using JP.Utils.UI;
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

        private Compositor _compositor;

        public VolListPage()
        {
            this.InitializeComponent();
            _volListPage = this;
            this._compositor = this.GetVisual().Compositor;
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
            MainNavigateToEvent(typeof(VolDetialPage));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var rootGrid = sender as Grid;

            rootGrid.PointerEntered += RootGrid_PointerEntered;
            rootGrid.PointerExited += RootGrid_PointerExited;
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rootGrid = sender as Grid;
            rootGrid.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, rootGrid.ActualWidth, rootGrid.ActualHeight)
            };
        }

        private void RootGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }
            var rootGrid = sender as Grid;
            var maskBorder = rootGrid as FrameworkElement;
            var img = rootGrid.Children[0] as FrameworkElement;

            ToggleItemPointOverAnimation(maskBorder, img, false);
        }

        private void RootGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }
            var rootGrid = sender as Grid;
            var maskBorder = rootGrid as FrameworkElement;
            var img = rootGrid.Children[0] as FrameworkElement;

            ToggleItemPointOverAnimation(maskBorder, img, true);
        }


        private void ToggleItemPointOverAnimation(FrameworkElement mask, FrameworkElement img, bool show)
        {
            var imgVisual = img.GetVisual();

            var scaleAnimation = CreateScaleAnimation(show);

            if (imgVisual.CenterPoint.X == 0 && imgVisual.CenterPoint.Y == 0)
            {
                imgVisual.CenterPoint = new Vector3((float)mask.ActualWidth / 2, (float)mask.ActualHeight / 2, 0f);
            }

            imgVisual.StartAnimation("Scale.x", scaleAnimation);
            imgVisual.StartAnimation("Scale.y", scaleAnimation);

        }

        private ScalarKeyFrameAnimation CreateScaleAnimation(bool show)
        {
            var scaleAnimation = _compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(1f, show ? 1.05f : 1f);
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(1000);
            scaleAnimation.StopBehavior = AnimationStopBehavior.LeaveCurrentValue;
            return scaleAnimation;
        }

        public void ScrollToTop()
        {
            VolListGridView.GetScrollViewer().ChangeView(null, 0, null);
        }
    }
}
