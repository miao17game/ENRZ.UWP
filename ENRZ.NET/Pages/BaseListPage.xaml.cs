using ENRZ.Core.Models;
using ENRZ.Core.Tools;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static ENRZ.NET.MainPage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ENRZ.NET.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BaseListPage : Page {
        public BaseListPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var args = e.Parameter as NavigateParameter;
            InnerResources.AddBaseListPageInstance(NaviPathTitle.RoutePath,this);
            HeaderResources.Source = args.Items;
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            GridViewResources.Source = null;
            var args = (sender as Pivot).SelectedItem as BarItemModel;
            if (args == null)
                return;
            NaviPathTitle.Route02 = args.Title=="首页"? null : args.Title;
            ChangeTitlePath(NaviPathTitle.RoutePath);
            ArgsPathKey = args.PathUri.ToString();
            if (InsideResources.IfContainsListInstance(ArgsPathKey)) {
                GridViewResources.Source = InsideResources.GetListInstance(ArgsPathKey);
                return;
            }
            if (InsideResources.IfContainsAGVInstance(ArgsPathKey))
                InsideResources.GetAGVInstance(ArgsPathKey).Opacity = 0;
            var newList = DataProcess.FetchNewsPreviewFromHtml(
                    (await WebProcess.GetHtmlResources(
                        ArgsPathKey, false))
                        .ToString());
            GridViewResources.Source = newList;
            InsideResources.GetAGVInstance(ArgsPathKey).Opacity = 1;
            InsideResources.AddResourcesInDec(ArgsPathKey, newList);
        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e) {

        }

        /// <summary>
        /// Resources Helper
        /// </summary>
        static class InsideResources {

            public static void AddResourcesInDec(string key, List<NewsPreviewModel> instance) { if (!ListMap.ContainsKey(key)) { ListMap.Add(key, instance); } }
            public static List<NewsPreviewModel> GetListInstance(string key) { return ListMap.ContainsKey(key) ? ListMap[key] : null; }
            public static bool IfContainsListInstance(string key) { return ListMap.ContainsKey(key); }
            static private Dictionary<string, List<NewsPreviewModel>> ListMap = new Dictionary<string, List<NewsPreviewModel>> {
            };

            public static void AddAGVInDec(string key, AdaptiveGridView instance) { if (!AdaptiveGridViewMap.ContainsKey(key)) { AdaptiveGridViewMap.Add(key, instance); } }
            public static AdaptiveGridView GetAGVInstance(string key) { return AdaptiveGridViewMap.ContainsKey(key) ? AdaptiveGridViewMap[key] : null; }
            public static bool IfContainsAGVInstance(string key) { return AdaptiveGridViewMap.ContainsKey(key); }
            static private Dictionary<string, AdaptiveGridView> AdaptiveGridViewMap = new Dictionary<string, AdaptiveGridView> {
            };

        }

        #region Properties and state
        public AdaptiveGridView adaptiveGV;
        private string ArgsPathKey;
        #endregion

    private void AdaptiveGridView_Loaded(object sender, RoutedEventArgs e) {
            adaptiveGV = sender as AdaptiveGridView;
            InsideResources.AddAGVInDec(ArgsPathKey, sender as AdaptiveGridView);
        }
    }
}
