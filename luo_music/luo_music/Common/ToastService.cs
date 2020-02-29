using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace LuoMusic.Common
{
    public class ToastService : Control
    {
        public static void SendToast(string text)
        {
            ToastService ts = new ToastService(text);
            if (ts._prepared)
            {
                _ = ts.ShowAsync();
            }
        }

        public static void SendToast(string text, TimeSpan time)
        {
            ToastService ts = new ToastService(text, time);
            if (ts._prepared)
            {
                _ = ts.ShowAsync();
            }
        }

        public static void SendToast(string text, int timeInMill)
        {
            ToastService ts = new ToastService(text, TimeSpan.FromMilliseconds(timeInMill));
            if (ts._prepared)
            {
                _ = ts.ShowAsync();
            }
        }

        #region DependencyProperty

        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        public static DependencyProperty ContentTextProperty = DependencyProperty.Register("ContentText",
            typeof(string), typeof(ToastService), new PropertyMetadata("Content"));

        public TimeSpan HideTimeSpan
        {
            get { return (TimeSpan)GetValue(HideTimeSpanProperty); }
            set { SetValue(HideTimeSpanProperty, value); }
        }

        public static readonly DependencyProperty HideTimeSpanProperty =
            DependencyProperty.Register("HideTimeSpan", typeof(TimeSpan), typeof(ToastService), new PropertyMetadata(TimeSpan.FromSeconds(1.0)));

        #endregion DependencyProperty

        private Page _currentPage;
        public Page CurrentPage
        {
            get
            {
                if (_currentPage != null) return _currentPage;
                else return ((Window.Current.Content as Frame).Content) as Page;
            }
            set
            {
                _currentPage = value;
            }
        }

        private readonly string _tempText;

        private Grid _rootGrid;
        private TextBlock _contentTB;
        private Storyboard _showStory;
        private Storyboard _hideStory;

        //Use popup to show the control
        private readonly Popup _currentPopup;

        //Provide the method to solve getting Storyboard before OnApplyTemplate() execute problem.
        private readonly TaskCompletionSource<int> _tcs;

        private bool _prepared;

        private ToastService()
        {
            if (CurrentPage == null)
            {
                return;
            }

            DefaultStyleKey = (typeof(ToastService));

            if (true)
            {
                _tcs = new TaskCompletionSource<int>();

                if (_currentPopup == null)
                {
                    _currentPopup = new Popup
                    {
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    this.Width = Window.Current.Bounds.Width;
                    this.Height = Window.Current.Bounds.Height;

                    _currentPopup.Child = this;
                    _currentPopup.IsOpen = true;
                }
            }

            CurrentPage.SizeChanged += Page_SizeChanged;
            _prepared = true;
        }

        private ToastService(string text) : this()
        {
            _tempText = text;
        }

        private ToastService(string text, TimeSpan time) : this()
        {
            _tempText = text;
            HideTimeSpan = time;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _ = UpdateSize();
        }

        private async Task UpdateSize()
        {
            await _tcs.Task;
            _rootGrid.Width = this.Width = Window.Current.Bounds.Width;
            _rootGrid.Height = this.Height = Window.Current.Bounds.Height;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitPane();
        }

        private void InitPane()
        {
            _contentTB = GetTemplateChild("ContentTB") as TextBlock;
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            _showStory = _rootGrid.Resources["ShowStory"] as Storyboard;
            _hideStory = _rootGrid.Resources["HideStory"] as Storyboard;
            _contentTB.Text = _tempText;
            _tcs.SetResult(0);
        }

        public async Task ShowAsync()
        {
            // todo: ugly
            await _tcs.Task;
            await UpdateSize();
            _showStory.Begin();
            await Task.Delay(HideTimeSpan);
            _hideStory.Begin();
            await Task.Delay(1000);
            _currentPopup.IsOpen = false;
            CurrentPage.SizeChanged -= Page_SizeChanged;
        }
    }
}
