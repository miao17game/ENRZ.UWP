using ENRZ.Core.Tools;
using ENRZ.NET.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ENRZ.NET {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        private const string HomeHost = "http://www.enrz.com/";
        private const string HomeHostInsert = "http://www.enrz.com";

        public MainPage() {
            this.InitializeComponent();
            PrepareFrame.Navigate(typeof(PreparePage));
            StatusBarInit.InitInnerDesktopStatusBar(true);
            Window.Current.SetTitleBar(BasePartBorder);
            GetResources();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            NavigationSplit.IsPaneOpen = !NavigationSplit.IsPaneOpen;
        }

        private async void GetResources() {
            var list = DataProcess.FetchNavigationBarFromHtml((await WebProcess.GetHtmlResources(HomeHost)).ToString());
            NaviBarResouces.Source = list;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {

        }
    }
}
