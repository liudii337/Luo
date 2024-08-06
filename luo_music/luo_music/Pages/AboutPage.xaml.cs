using JP.Utils.Debug;
using JP.Utils.Helper;
using LuoMusic.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace LuoMusic.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : NavigableUserControl
    {
        public AboutPage()
        {
            this.InitializeComponent();
            VersionTB.Text = App.GetAppVersion();
            SetLocalizedRichText();
        }

        private void SetLocalizedRichText()
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            var localizedText = resourceLoader.GetString("CreditsString");

            // 将字符串转换为 XAML 片段
            Paragraph paragraph = (Paragraph)XamlReader.Load(localizedText);

            // 清空现有内容并添加新的段落
            richTextBlock.Blocks.Clear();
            richTextBlock.Blocks.Add(paragraph);
        }

        private async void MailFeedback_Click(object sender, RoutedEventArgs e)
        {
            EmailRecipient rec = new EmailRecipient("1120353795@qq.com");
            EmailMessage mes = new EmailMessage();
            mes.To.Add(rec);
            var attach = await Logger.GetLogFileAttachementAsync();
            if (attach != null)
            {
                mes.Attachments.Add(attach);
            }
            var platform = DeviceHelper.IsDesktop ? "PC" : "Mobile";
            var Version = App.GetAppVersion();

            mes.Subject = $"Luoo for Windows 10 {platform}, {Version} feedback, {DeviceHelper.OSVersion}";
            mes.Body = "在这里键入你在使用中遇到的问题：\n1. \n2. \n\n";

            await EmailManager.ShowComposeNewEmailAsync(mes);
        }

        private async void GitHubFeedback_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/liudii337/Luo"));
        }

        private async void RateBtn_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?PFN=" + Package.Current.Id.FamilyName));
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Presented = false;
        }
    }
}
