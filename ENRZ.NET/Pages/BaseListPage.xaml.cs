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

using static ENRZ.Core.Tools.UWPStates;
using static ENRZ.NET.Pages.BaseListPage.InsideResources;

namespace ENRZ.NET.Pages {
    
    public sealed partial class BaseListPage : Page {

        #region Constructor
        public BaseListPage() {
            this.InitializeComponent();
        }
        #endregion

        #region Events
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var args = e.Parameter as NavigateParameter;
            MainPage.InnerResources.AddBaseListPageInstance(MainPage.NaviPathTitle.RoutePath,this);
            HeaderResources.Source = args.Items;
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            MainPage.Current.BaseListRing.IsActive = true;
            GridViewResources.Source = null;
            var args = (sender as Pivot).SelectedItem as BarItemModel;
            if (args == null) {
                MainPage.Current.BaseListRing.IsActive = false;
                return;
            }
            MainPage.ChangeTitlePath(3, (sender as Pivot).SelectedIndex == 0 ? null : args.Title);
            ArgsPathKey = args.PathUri.ToString();
            if (IfContainsListInstance(ArgsPathKey)) {
                GridViewResources.Source = GetListInstance(ArgsPathKey);
                MainPage.Current.BaseListRing.IsActive = false;
                return;
            }
            if (IfContainsAGVInstance(ArgsPathKey))
                GetAGVInstance(ArgsPathKey).Opacity = 0;
            var newList = DataProcess.FetchNewsPreviewFromHtml(
                    (await WebProcess.GetHtmlResources(
                        ArgsPathKey, false))
                        .ToString());
            GridViewResources.Source = newList;
            GetAGVInstance(ArgsPathKey).Opacity = 1;
            AddResourcesInDec(ArgsPathKey, newList);
            MainPage.Current.BaseListRing.IsActive = false;
        }

        private void AdaptiveGridView_Loaded(object sender, RoutedEventArgs e) {
            adaptiveGV = sender as AdaptiveGridView;
            AddAGVInDec(ArgsPathKey, sender as AdaptiveGridView);
        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var model = e.ClickedItem as NewsPreviewModel;
            if (model == null)
                return;
            MainPage.Current.NavigateToBase?.Invoke(
                sender,
                new NavigateParameter { PathUri = model.PathUri },
                MainPage.InnerResources.GetFrameInstance(NavigateType.Content),
                MainPage.InnerResources.GetPageType(NavigateType.Content));
        }
        #endregion

        #region Methods

        #endregion

        #region Inside Resources class
        /// <summary>
        /// Resources Helper
        /// </summary>
        internal static class InsideResources {

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
        #endregion

        #region Properties and state
        public AdaptiveGridView adaptiveGV;
        private string ArgsPathKey;
        #endregion

    }
}
