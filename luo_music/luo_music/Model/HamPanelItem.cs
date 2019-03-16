using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;

namespace luo_music.Model
{
    public class HamPanelItem
    {
        public VirtualKey Index { get; set; }
        public string IndexNum { get; set; }

        public string Title { get; set; }

        public Type TargetType { get; set; }

        public string Icon { get; set; }

        public Uri BG { get; set; }

        public FontWeight ChangeWeight(bool b)
        {
            return b ? FontWeights.Bold : FontWeights.Normal;
        }

        //public SolidColorBrush ChangeForeground(bool b)
        //{
        //    return (SolidColorBrush)(b ? MainPage.Current.Resources["AccentForText"] : MainPage.Current.Resources["SystemControlForegroundBaseHighBrush"]);
        //}

        //public SolidColorBrush ChangeTextForeground(bool b)
        //{
        //    return (SolidColorBrush)(b ? MainPage.Current.Resources["SystemControlForegroundBaseHighBrush"] : MainPage.Current.Resources["ButtonDisabledForegroundThemeBrush"]);
        //}

        public double BoolToOpacity(bool b)
        {
            return b ? 1.0 : 0.333333333333;
        }

        public double PaneLength(bool a)
        {
            return a ? 320d : 48d;
        }
    }
}
