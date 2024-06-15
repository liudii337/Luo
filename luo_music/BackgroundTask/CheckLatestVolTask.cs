using Luo.Shared.Data;
using Luo.Shared.Helper;
using Luo.Shared.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class CheckLatestVolTask : IBackgroundTask
    {
        public ApplicationDataContainer LocalSettings { get; set; }

        private const string KEY = "BackgroundWallpaperSource";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("===========background task run==============");
            var defer = taskInstance.GetDeferral();
            //获取最新一期的Vol概况和序号
            var VolService = new VolService(Request.GetAllVol_q,new LuoVolFactory(), CancellationTokenSourceFactory.CreateDefault());
            var LatestVol = await VolService.GetLatestVolAsync();
            //比较之前存储的最新号码，确定是否通知
            LocalSettings = ApplicationData.Current.LocalSettings;

            var LatestNumStorage = (string)LocalSettings.Values["LatestVolNum"];
            if(Convert.ToInt16(LatestVol.VolNum)> Convert.ToInt16(LatestNumStorage))
            {
                // 创建一个Toast通知
                ToastNotification toast = ToastNotificationHelper.CreateToastNotification(LatestVol.VolNum, LatestVol.Title, LatestVol.Description, LatestVol.Cover);

                // 显示Toast通知
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            defer.Complete();
        }
    }
}
