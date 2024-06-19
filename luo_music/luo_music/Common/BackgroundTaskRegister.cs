using BackgroundTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace LuoMusic.Common
{
    public static class BackgroundTaskRegister
    {
        private static string NAME => "CheckLatestVolTask";
        private static uint PERIOD_HOUR_MINS => 60 ;

        public static async Task RegisterAsync()
        {
            if (IsBackgroundTaskRegistered())
            {
                Debug.WriteLine("IsBackgroundTaskRegistered: true");
                return;
            }
            var period = PERIOD_HOUR_MINS * 4;
            await RegisterBackgroundTask(typeof(CheckLatestVolTask),
                                                    new TimeTrigger(period, false),
                                                    null);
        }

        public static async Task UnregisterAsync()
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status != BackgroundAccessStatus.AlwaysAllowed
                && status != BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                ToastService.SendToast("BackgroundRegisterFailed", TimeSpan.FromMilliseconds(5000));
                return;
            }

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == NAME)
                {
                    cur.Value.Unregister(true);
                }
            }

            Debug.WriteLine($"===================unregistered===================");
        }

        private static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(Type taskEntryPoint,
                                                                IBackgroundTrigger trigger,
                                                                IBackgroundCondition condition)
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status != BackgroundAccessStatus.AlwaysAllowed
                && status != BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                ToastService.SendToast("BackgroundRegisterFailed", TimeSpan.FromMilliseconds(5000));
                return null;
            }

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == NAME)
                {
                    cur.Value.Unregister(true);
                }
            }

            var builder = new BackgroundTaskBuilder
            {
                Name = NAME,
                TaskEntryPoint = taskEntryPoint.FullName
            };

            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            Debug.WriteLine($"===================Task {NAME} registered successfully===================");

            return task;
        }

        private static bool IsBackgroundTaskRegistered()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == NAME)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
