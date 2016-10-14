using Edi.UWP.Helpers;
using ENRZ.Core.Helpers;
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

using static ENRZ.Core.Tools.UWPStates;
using static ENRZ.NET.Pages.SettingsPage.InsideResources;

namespace ENRZ.NET.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page {
        public SettingsPage() {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            VersionMessage.Text = GetUIString("VersionMessage") + Utils.GetAppVersion();
            ThemeSwitch.IsOn = (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsDarkThemeOrNot) ?? true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            MainPage.ChangeTitlePath(2, GetUIString("SettingsString"));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if ((sender as Pivot).SelectedIndex == 0) {
                MainPage.ChangeTitlePath(3, null);
                return;
            }
            MainPage.ChangeTitlePath(
                3, (e.AddedItems.FirstOrDefault() as PivotItem).Header as string != GetUIString("SettingsString") ? 
                (e.AddedItems.FirstOrDefault() as PivotItem).Header as string :
                null);
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

            string subject = GetUIString("Feedback_Subject");
            string body = $"{GetUIString("Feedback_Body")}：{msg}  " +
                          $"（{GetUIString("Feedback_Version")}：{Utils.GetAppVersion()} ";

            if (includeDeviceInfo) {
                body += $", {GetUIString("Feedback_FriendlyName")}：{deviceInfo.FriendlyName}, " +
                          $"{GetUIString("Feedback_OS")}：{deviceInfo.OperatingSystem}, " +
                          $"SKU：{deviceInfo.SystemSku}, " +
                          $"{GetUIString("Feedback_SPN")}：{deviceInfo.SystemProductName}, " +
                          $"{GetUIString("Feedback_SMF")}：{deviceInfo.SystemManufacturer}, " +
                          $"{GetUIString("Feedback_SFV")}：{deviceInfo.SystemFirmwareVersion}, " +
                          $"{GetUIString("Feedback_SHV")}：{deviceInfo.SystemHardwareVersion}）";
            } else {
                body += ")";
            }

            string to = "miao17game@qq.com";
            await Tasks.OpenEmailComposeAsync(to, subject, body);
        }

        private void ThemeSwitch_Toggled(object sender, RoutedEventArgs e) {
            GetSwitchHandler((sender as ToggleSwitch).Name)
               .Invoke((sender as ToggleSwitch).Name);
        }

        private void OnThemeSwitchToggled(ToggleSwitch sender) {
            SettingsHelper.SaveSettingsValue(SettingsConstants.IsDarkThemeOrNot, sender.IsOn);
            MainPage.Current.RequestedTheme = sender.IsOn ? ElementTheme.Dark : ElementTheme.Light;
        }

        internal static class InsideResources {

            public static ToggleSwitch GetSwitchInstance(string str) { return SwitchSettingsMaps.ContainsKey(str) ? SwitchSettingsMaps[str] : null; }
            static private Dictionary<string, ToggleSwitch> SwitchSettingsMaps = new Dictionary<string, ToggleSwitch> {
            {Current.ThemeSwitch.Name,Current.ThemeSwitch},
        };

            public static SwitchEventHandler GetSwitchHandler(string str) { return SwitchHandlerMaps.ContainsKey(str) ? SwitchHandlerMaps[str] : null; }
            static private Dictionary<string, SwitchEventHandler> SwitchHandlerMaps = new Dictionary<string, SwitchEventHandler> {
            {Current.ThemeSwitch.Name, new SwitchEventHandler(instance=> { Current.OnThemeSwitchToggled(GetSwitchInstance(instance)); }) },
        };

        }

        #region Properties and state
        public static SettingsPage Current;
        public delegate void SwitchEventHandler(string instance);
        #endregion
    }
}
