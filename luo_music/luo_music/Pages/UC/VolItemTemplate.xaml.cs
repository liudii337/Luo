using LuoMusic.Common.Composition;
using LuoMusic.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace LuoMusic.Pages.UC
{
    public sealed partial class VolItemTemplate : UserControl
    {
        public VolItem volItem { get { return this.DataContext as VolItem; } }

        private Compositor _compositor;

        public VolItemTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += VolItemTemplate_DataContextChanged;
            this._compositor = this.GetVisual().Compositor;
        }

        private void VolItemTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var buttonVisual = HeartButton.GetVisual();
            if (volItem != null)
            {
                if (volItem.IsHeartVol)
                {
                    buttonVisual.Opacity = 1f;
                }
                else
                {
                    buttonVisual.Opacity = 0f;
                }
            }
            //Bindings.Update();
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // Set Maskborder
            var maskVisual = MaskBorder.GetVisual();
            maskVisual.Opacity = 0f;
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RootGrid.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, RootGrid.ActualWidth, RootGrid.ActualHeight)
            };
        }

        private void RootGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }

            ToggleItemPointOverAnimation(MaskBorder, Cover, HeartButton, volItem, true);
        }

        private void RootGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }

            ToggleItemPointOverAnimation(MaskBorder, Cover, HeartButton, volItem, false);
        }

        private void ToggleItemPointOverAnimation(FrameworkElement mask, FrameworkElement img, FrameworkElement btn, VolItem volItem, bool show)
        {
            var maskVisual = mask.GetVisual();
            var imgVisual = img.GetVisual();
            var btnVisual = btn.GetVisual();

            var fadeAnimation = CreateFadeAnimation(show);
            var scaleAnimation = CreateScaleAnimation(show);

            if (imgVisual.CenterPoint.X == 0 && imgVisual.CenterPoint.Y == 0)
            {
                imgVisual.CenterPoint = new Vector3((float)mask.ActualWidth / 2, (float)mask.ActualHeight / 2, 0f);
            }

            if (!volItem.IsHeartVol)
            {
                btnVisual.StartAnimation("Opacity", fadeAnimation);
            }

            maskVisual.StartAnimation("Opacity", fadeAnimation);

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

        private ScalarKeyFrameAnimation CreateFadeAnimation(bool show)
        {
            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, show ? 1f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);

            return fadeAnimation;
        }
    }
}
