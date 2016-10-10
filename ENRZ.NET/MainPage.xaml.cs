using ENRZ.Core.Models;
using ENRZ.Core.Tools;
using ENRZ.NET.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Current = this;
            NavigateTitlePath = this.navigateTitlePath;
            ChangeTitlePath(NaviPathTitle.RoutePath);
            PrepareFrame.Navigate(typeof(PreparePage));
            StatusBarInit.InitInnerDesktopStatusBar(true);
            Window.Current.SetTitleBar(BasePartBorder);
            GetResources();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            NavigationSplit.IsPaneOpen = !NavigationSplit.IsPaneOpen;
        }

        private async void GetResources() {
            var list = DataProcess.FetchNavigationBarFromHtml((await WebProcess.GetHtmlResources(HomeHost, false)).ToString());
            NaviBarResouces.Source = list;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            BasePartFrame.Navigate(typeof(SettingsPage));
            HamburgerListBox.SelectedIndex = -1;
        }

        private void HamburgerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            NavigationSplit.IsPaneOpen = false;
            var model = e.AddedItems.FirstOrDefault() as NavigationBarModel;
            if (model == null)
                return;
            NaviPathTitle.Route01 = model.Title;
            ChangeTitlePath(NaviPathTitle.RoutePath);
            NavigateType type = model.Title.ToString() == "美图" ?
            NavigateType.SpecialImage :
            NavigateType.NaviBar;
            if (InnerResources.IfContainsPageInstance(NaviPathTitle.RoutePath)) {
                InnerResources.GetFrameInstance(type).Content = InnerResources.GetPageInstance(NaviPathTitle.RoutePath);
            } else {
                NavigateToBase?.Invoke(
                    sender,
                    new NavigateParameter { PathUri = model.PathUri, Items = model.Items, },
                    InnerResources.GetFrameInstance(type),
                    InnerResources.GetPageType(type));
            }
        }

        public static void ChangeTitlePath(string value) {
            Current.NavigateTitlePath.Text = value;
        }

        /// <summary>
        /// Resources Helper
        /// </summary>
        public static class InnerResources{

            public static Type GetPageType(NavigateType type) { return PagesMaps.ContainsKey(type) ? PagesMaps[type] : null; }
            static private Dictionary<NavigateType, Type> PagesMaps = new Dictionary<NavigateType, Type> {
            {NavigateType.NaviBar,typeof(BaseListPage)},
            {NavigateType.InnerBarItem,typeof(ContentPage)},
            {NavigateType.SpecialImage,typeof(ImagePage)},
            {NavigateType.Settings,typeof(SettingsPage)},
        };

            public static Frame GetFrameInstance(NavigateType type) { return FrameMaps.ContainsKey(type) ? FrameMaps[type] : null; }
            static private Dictionary<NavigateType, Frame> FrameMaps = new Dictionary<NavigateType, Frame> {
            {NavigateType.NaviBar,Current.BasePartFrame},
            {NavigateType.SpecialImage,Current.BasePartFrame},
            {NavigateType.InnerBarItem,Current.ContentFrame},
        };

            public static void AddBaseListPageInstance(string key, BaseListPage instance) { if (!BaseListPageMap.ContainsKey(key)) { BaseListPageMap.Add(key, instance); } }
            public static BaseListPage GetPageInstance(string key) { return BaseListPageMap.ContainsKey(key) ? BaseListPageMap[key] : null; }
            public static bool IfContainsPageInstance(string key) { return BaseListPageMap.ContainsKey(key); }
            static private Dictionary<string, BaseListPage> BaseListPageMap = new Dictionary<string, BaseListPage> {
            };

        }

        #region Properties and state
        public static MainPage Current;
        public TextBlock NavigateTitlePath;
        public delegate void NavigationEventHandler(object sender, NavigateParameter parameter, Frame frame, Type type);
        public NavigationEventHandler NavigateToBase = (sender, parameter, frame, type) => { frame.Navigate(type, parameter); };
        public struct PathTitle {
            public string Route01 { get; set; }
            public string Route02 { get; set; }
            public string RoutePath { get { return Route02!=null? Route01 + "-" + Route02: Route01; } }
        }
        public static PathTitle NaviPathTitle = new PathTitle { Route01 = "ENRZ.COM" };
        #endregion
    }
}
