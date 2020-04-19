using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Luo.Shared.Helper
{
    public static class Tools
    {
        public static bool AlmostEqualTo(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.0000001;
        }

        public static void SetStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    titleBar.ButtonForegroundColor = Colors.Black;

                    coreTitleBar.ExtendViewIntoTitleBar = true;
                    var applicationView = ApplicationView.GetForCurrentView();
                    applicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                }
            }

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.Black;
                statusBar.BackgroundOpacity = 0;

                var applicationView = ApplicationView.GetForCurrentView();
                applicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            }
        }


    }
}
