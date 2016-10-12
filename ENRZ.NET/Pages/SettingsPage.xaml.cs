using Edi.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ENRZ.NET.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page {
        public SettingsPage() {
            this.InitializeComponent();
            VersionMessage.Text = "版本信息：" + Utils.GetAppVersion();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private async void FeedBackBtn_Click(object sender, RoutedEventArgs e) {
            await ReportError(null, "N/A", true);
        }

        /// <summary>
        /// ReportError Method
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="msg"></param>
        /// <param name="pageSummary"></param>
        /// <param name="includeDeviceInfo"></param>
        /// <returns></returns>
        public static async Task ReportError(string msg = null, string pageSummary = "N/A", bool includeDeviceInfo = true) {
            var deviceInfo = new EasClientDeviceInformation();

            string subject = "ENRZ.COM 反馈";
            string body = $"问题描述：{msg}  " +
                          $"（程序版本：{Utils.GetAppVersion()} ";

            if (includeDeviceInfo) {
                body += $", \n设备名：{deviceInfo.FriendlyName}, " +
                          $"\n操作系统：{deviceInfo.OperatingSystem}, " +
                          $"\nSKU：{deviceInfo.SystemSku}, " +
                          $"\n产品名称：{deviceInfo.SystemProductName}, " +
                          $"\n制造商：{deviceInfo.SystemManufacturer}, " +
                          $"\n固件版本：{deviceInfo.SystemFirmwareVersion}, " +
                          $"\n硬件版本：{deviceInfo.SystemHardwareVersion}）";
            } else {
                body += ")";
            }

            string to = "miao17game@qq.com";
            await Tasks.OpenEmailComposeAsync(to, subject, body);
        }
    }
}
