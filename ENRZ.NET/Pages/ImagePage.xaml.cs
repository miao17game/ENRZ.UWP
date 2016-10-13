using ENRZ.Core.DataVirtualization;
using ENRZ.Core.Models;
using ENRZ.Core.Tools;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static ENRZ.Core.Tools.UWPStates;
using static ENRZ.NET.Pages.ImagePage.InsideResources;

namespace ENRZ.NET.Pages {
   
    public sealed partial class ImagePage : Page {

        #region Constructor
        public ImagePage() {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        #endregion

        #region Events
        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            MainPage.Current.BaseListRing.IsActive = true;
            if (NavigateState == PageState.Navigated) {
                DoWorkAsync(()=> { SetButtonToggled(nowToggledButton); });
                MainPage.Current.BaseListRing.IsActive = false;
                return;
            }
            DoWorkAsync(() => { SetButtonToggled(IndexButton.Name); });
            var args = e.Parameter as NavigateParameter;
            var source = DataProcess.FetchImagesIndexFromHtml(
                    (await WebProcess.GetHtmlResources(
                        args.PathUri.ToString(), true))
                        .ToString());
            SetIndexRangeResources(source);
            MainPage.Current.BaseListRing.IsActive = false;
        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var model = e.ClickedItem as SimpleImgModel;
            if (model == null)
                return;
            MainPage.Current.NavigateToBase?.Invoke(
                sender,
                new NavigateParameter { PathUri = model.PathUri },
                MainPage.InnerResources.GetFrameInstance(NavigateType.PicutreContent),
                MainPage.InnerResources.GetPageType(NavigateType.PicutreContent));
        }

        private async void ButtonSelect(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            DoWorkAsync(() => { SetButtonToggled(button.Name); });
            MainPage.Current.BaseListRing.IsActive = true;
            if (button.Name == IndexButton.Name) {
                SetVisibility(IndexScroll, true);
                MainPage.ChangeTitlePath(3, null);
                MainPage.Current.BaseListRing.IsActive = false;
                return;
            }
            SetVisibility(IndexScroll, false);
            htmlResourcesNow = (await WebProcess.GetHtmlResources(GetUriPath(button.Name), true)).ToString();
            var headList = DataProcess.FetchBarItemNaviFromHtml(htmlResourcesNow);
            HeaderResources.Source = headList;
            MainPage.Current.BaseListRing.IsActive = false;
        }

        private void MyFlip_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void FlipInnerButton_Click(object sender, RoutedEventArgs e) {
            var uri = (sender as Button).CommandParameter as Uri;
            if (uri == null)
                return;
            MainPage.Current.NavigateToBase?.Invoke(
                sender,
                new NavigateParameter { PathUri = uri },
                MainPage.InnerResources.GetFrameInstance(NavigateType.PicutreContent),
                MainPage.InnerResources.GetPageType(NavigateType.PicutreContent));
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            var model = e.ClickedItem as SimpleImgModel;
            if (model == null)
                return;
            MainPage.Current.NavigateToBase?.Invoke(
                sender,
                new NavigateParameter { PathUri = model.PathUri },
                MainPage.InnerResources.GetFrameInstance(NavigateType.PicutreContent),
                MainPage.InnerResources.GetPageType(NavigateType.PicutreContent));
        }

        private void MyFlip_Loaded(object sender, RoutedEventArgs e) {
            InitFlipTimer();
        }

        private void IndexGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var model = e.ClickedItem as SimpleImgModel;
            if (model == null)
                return;
            MainPage.Current.NavigateToBase?.Invoke(
                sender, 
                new NavigateParameter { PathUri = model.PathUri }, 
                MainPage.InnerResources.GetFrameInstance(NavigateType.PicutreContent), 
                MainPage.InnerResources.GetPageType(NavigateType.PicutreContent));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            MainPage.Current.BaseListRing.IsActive = true;
            GridViewResources.Source = null;
            var args = (sender as Pivot).SelectedItem as BarItemModel;
            if (args == null) {
                MainPage.Current.BaseListRing.IsActive = false;
                return;
            }
            MainPage.ChangeTitlePath(3, args.Title);
            ArgsPathKey = args.Title;
            GridViewResources.Source = IfContainsResources(ArgsPathKey) ?
                GetResources(ArgsPathKey) :
                SetAndSaveResources(args);
            MainPage.Current.BaseListRing.IsActive = false;
        }

        private void AdaptiveGridView_Loaded(object sender, RoutedEventArgs e) {
            adaptiveGV = sender as AdaptiveGridView;
            AddAGVInDec(ArgsPathKey, sender as AdaptiveGridView);
        }

        #endregion

        #region Methods

        private async void DoWorkAsync(Action action) {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { action.Invoke(); });
        }

        private void SetIndexRangeResources(NewsPreviewModel source) {
            FlipResouces.Source = source.SlideImageList;
            TopResouces.Source = source.TopImageList;
            RecommendResouces.Source = source.RecommendImageList;
            SelectResouces.Source = source.SelectImageList;
            GirlResouces.Source = source.GirlImageList;
            FashionResouces.Source = source.FashionImageList;
            PlaythingResouces.Source = source.PlaythingImageList;
            EntResouces.Source = source.EntImageList;
            NavigateState = PageState.Navigated;
            flipSourceCount = source.SlideImageList.Count;
        }

        private void InitFlipTimer() {
            try {
                if (timer != null && timer.IsEnabled)
                    timer.Stop();
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 5);
                timer.Tick += (obj, args) => {
                    try {
                        if (MyFlip.SelectedIndex < flipSourceCount - 1)
                            MyFlip.SelectedIndex++;
                        else
                            MyFlip.SelectedIndex = 0;
                    } catch { /* something goes wrong if action done when resources does not loaded.*/}
                };
                timer.Start();
            } catch { /* something goes wrong if action done when resources does not loaded.*/}
        }

        private ENRZDataContext<SimpleImgModel> SetAndSaveResources(BarItemModel args) {
            if (IfContainsAGVInstance(ArgsPathKey))
                GetAGVInstance(ArgsPathKey).Opacity = 0;
            var resources = new ENRZDataContext<SimpleImgModel>(
                        FetchMoreResources,
                        24,
                        args.PathUri.ToString(),
                        InitSelector.Special);
            if (IfContainsAGVInstance(ArgsPathKey))
                GetAGVInstance(ArgsPathKey).Opacity = 1;
            AddResources(args.Title, resources);
            return ListResMap[args.Title];
        }

        private async Task<List<SimpleImgModel>> FetchMoreResources(int number, string pathString, uint rollNum, uint nowWholeCountX) {
            var Host = pathString + "{0}.shtml";
            if (nowWholeCountX / 24 != 1)
                Host = string.Format(Host, nowWholeCountX / 24);
            else
                Host = pathString;
            var result = DataProcess.FetchBarItemInnerFromHtml((await WebProcess.GetHtmlResources(Host, true)).ToString());
            if (result.Count == 0) { DataProcess.ReportException("没有更多内容了"); await Task.Delay(5000); }
            return result;
        }

        #endregion

        #region Inside Resources class
        internal static class InsideResources {

            public static string GetSceneName(string buttonName) { return SceneMap.ContainsKey(buttonName) ? SceneMap[buttonName] : null; }
            static private Dictionary<string, string> SceneMap = new Dictionary<string, string> {
                {Current.IndexButton.Name, "首页" },
                {Current.GirlButton.Name, "美女" },
                {Current.FashionlButton.Name, "时尚" },
                {Current.PalythingButton.Name, "玩物" },
                {Current.EntButton.Name, "娱乐" },
            };

            public static Button GetButtonFromMoreName(string buttonName) { return ButtonMap.ContainsKey(buttonName) ? ButtonMap[buttonName] : null; }
            public static void SetButtonToggled(string buttonName) {
                ClearAllButtonToggled();
                GetButtonFromMoreName(buttonName).Foreground = Application.Current.Resources["ENRZForeground02"] as Brush;
                Current.nowToggledButton = buttonName;
            }
            public static void ClearAllButtonToggled() {
                var solidBrush = MainPage.Current.RequestedTheme == ElementTheme.Dark ?
                        new SolidColorBrush(Colors.White) :
                        new SolidColorBrush(Color.FromArgb(255, 70, 70, 70));
                ButtonMap.Values.Distinct().ToList().ForEach(button => button.Foreground = solidBrush);
            }
            static private Dictionary<string, Button> ButtonMap = new Dictionary<string, Button> {
                {Current.IndexButton.Name, Current.IndexButton },
                {Current.GirlButton.Name, Current.GirlButton },
                {Current.GirlMoreButton.Name, Current.GirlButton },
                {Current.FashionlButton.Name, Current.FashionlButton },
                {Current.FashionMoreButton.Name, Current.FashionlButton },
                {Current.PalythingButton.Name, Current.PalythingButton },
                {Current.PlaythingMoreButton.Name, Current.PalythingButton },
                {Current.EntButton.Name, Current.EntButton },
                {Current.EntMoreButton.Name, Current.EntButton },
            };

            public static string GetUriPath(string key) { return UriPathMap.ContainsKey(key) ? UriPathMap[key] : null; }
            static private Dictionary<string, string> UriPathMap = new Dictionary<string, string> {
                {Current.IndexButton.Name, "http://pic.enrz.com/" },
                {Current.GirlButton.Name, "http://pic.enrz.com/miss/" },
                { Current.GirlMoreButton.Name, "http://pic.enrz.com/miss/" },
                {Current.FashionlButton.Name, "http://pic.enrz.com/fashion/" },
                {Current.FashionMoreButton.Name, "http://pic.enrz.com/fashion/" },
                {Current.PalythingButton.Name, "http://pic.enrz.com/plaything/" },
                {Current.PlaythingMoreButton.Name, "http://pic.enrz.com/plaything/" },
                {Current.EntButton.Name, "http://pic.enrz.com/ent/" },
                {Current.EntMoreButton.Name, "http://pic.enrz.com/ent/" },
            };

            public static ENRZDataContext<SimpleImgModel> GetResources(string key) { return ListResourcesMap.ContainsKey(key) ? ListResourcesMap[key] : null; }
            public static void AddResources(string key, ENRZDataContext<SimpleImgModel> value) { if (!ListResourcesMap.ContainsKey(key)) ListResourcesMap.Add(key, value); } 
            public static bool IfContainsResources(string key) { return ListResourcesMap.ContainsKey(key); }
            public static Dictionary<string, ENRZDataContext<SimpleImgModel>> ListResMap { get { return ListResourcesMap; } }
            static private Dictionary<string, ENRZDataContext<SimpleImgModel>> ListResourcesMap = new Dictionary<string, ENRZDataContext<SimpleImgModel>> {
            };

            public static void AddAGVInDec(string key, AdaptiveGridView instance) { if (!AdaptiveGridViewMap.ContainsKey(key)) { AdaptiveGridViewMap.Add(key, instance); } }
            public static AdaptiveGridView GetAGVInstance(string key) { return AdaptiveGridViewMap.ContainsKey(key) ? AdaptiveGridViewMap[key] : null; }
            public static bool IfContainsAGVInstance(string key) { return AdaptiveGridViewMap.ContainsKey(key); }
            static private Dictionary<string, AdaptiveGridView> AdaptiveGridViewMap = new Dictionary<string, AdaptiveGridView> {
            };

        }

        #endregion

        #region Properties and state
        public static ImagePage Current;
        private DispatcherTimer timer;
        private int flipSourceCount;
        private string htmlResourcesNow;
        private string ArgsPathKey;
        private string nowToggledButton;
        public AdaptiveGridView adaptiveGV;
        private PageState NavigateState = PageState.Default;
        public enum PageState { Default = 0, Navigated = 1,}
        #endregion

    }
}
