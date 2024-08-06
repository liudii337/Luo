using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;

namespace Luo.Shared.Helper
{
    public static class ToastNotificationHelper
    {
        public static ToastNotification CreateToastNotification(string volnum, string title, string description, string cover)
        {
            ToastContent toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Vol."+volnum+" "+title
                            },

                            new AdaptiveText()
                            {
                                Text = description
                            },
                        },

                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = cover,
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton(ResourcesHelper.GetResString("ListenNow"), "luoo://?volnum="+volnum)
                        { ActivationType = ToastActivationType.Protocol },
                        new ToastButton(ResourcesHelper.GetResString("LaterListen"),"").SetDismissActivation()
                    }
                },
                Launch= "?volnum=" + volnum
            };

            return new ToastNotification(toastContent.GetXml());
        }
    }
}
