using LuoMusic.Common.Composition;
using LuoMusic.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LuoMusic.Common
{
    public class PresentedArgs
    {
        public bool Presented { get; set; }

        public PresentedArgs(bool presented)
        {
            Presented = presented;
        }
    }

    public class NavigableUserControl : UserControl, INavigableUserControl
    {
        public bool Presented
        {
            get { return (bool)GetValue(PresentedProperty); }
            set { SetValue(PresentedProperty, value); }
        }

        public static readonly DependencyProperty PresentedProperty =
            DependencyProperty.Register("Presented", typeof(bool), typeof(NavigableUserControl),
                new PropertyMetadata(false, OnPresentedPropertyChanged));

        public event EventHandler<PresentedArgs> OnPresentedChanged;

        private static void OnPresentedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as INavigableUserControl;
            if ((bool)e.NewValue)
            {
                control.OnPresented();
            }
            else
            {
                control.OnHide();
            }

            control.ToggleAnimation();
        }

        private Compositor _compositor;
        private Visual _rootVisual;

        public NavigableUserControl()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                InitComposition();
                this.SizeChanged += UserControlBase_SizeChanged;
            }
        }

        private void UserControlBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetOffset();
        }

        private void InitComposition()
        {
            _compositor = this.GetVisual().Compositor;
            _rootVisual = this.GetVisual();
            ResetOffset();
        }

        private void ResetOffset()
        {
            if (!Presented)
            {
                _rootVisual.SetTranslation(new Vector3(0f, (float)this.ActualHeight, 0f));
            }
        }

        public virtual void OnHide()
        {
            OnPresentedChanged?.Invoke(this, new PresentedArgs(false));
        }

        public virtual void OnPresented()
        {
            OnPresentedChanged?.Invoke(this, new PresentedArgs(true));
        }

        public void ToggleAnimation()
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, Presented ? 0f : (float)this.ActualHeight);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(800);

            _rootVisual.StartAnimation(_rootVisual.GetTranslationYPropertyName(), offsetAnimation);
        }
    }
}
