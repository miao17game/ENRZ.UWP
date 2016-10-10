using ENRZ.Core.Models;
using ENRZ.Core.Tools;
using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ENRZ.NET.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagePage : Page {
        public ImagePage() {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var args = e.Parameter as NavigateParameter;
            HeaderResources.Source = args.Items;
            var source = DataProcess.FetchImagesIndexFromHtml(
                    (await WebProcess.GetHtmlResources(
                        args.PathUri.ToString(), true))
                        .ToString());
            FlipResouces.Source = source.SlideImageList;
            TopResouces.Source = source.TopImageList;
            RecommendResouces.Source = source.RecommendImageList;
            SelectResouces.Source = source.SelectImageList;
            GirlResouces.Source = source.GirlImageList;
            FashionResouces.Source = source.FashionImageList;
            PlaythingResouces.Source = source.PlaythingImageList;
            EntResouces.Source = source.EntImageList;
        }

        private void InitFlipTimer() {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 7);
            timer.Tick += (obj, args) => {
                if (MyFlip.SelectedIndex < 4)
                    MyFlip.SelectedIndex++;
                else
                    MyFlip.SelectedIndex = 0;
            };
            timer.Start();
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var args = (sender as Pivot).SelectedItem as BarItemModel;
            if (args == null)
                return;
            GridViewResources.Source =
                DataProcess.FetchNewsPreviewFromHtml(
                    (await WebProcess.GetHtmlResources(
                        args.PathUri.ToString(), false))
                        .ToString());
        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e) {

        }

        private void ButtonSelect(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if ( button.Name== "IndexButton" ) {
                IndexScroll.Visibility = Visibility.Visible;
                return;
            }
            IndexScroll.Visibility = Visibility.Collapsed;

        }

        private void MyFlip_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void FlipInnerButton_Click(object sender, RoutedEventArgs e) {

        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {

        }

        private void MyFlip_Loaded(object sender, RoutedEventArgs e) {
            InitFlipTimer();
        }

        private void IndexGridView_ItemClick(object sender, ItemClickEventArgs e) {

        }

        static class InnerResources {

            public static string GetSceneName(string buttonName) { return SceneMap.ContainsKey(buttonName) ? SceneMap[buttonName] : null; }
            static private Dictionary<string, string> SceneMap = new Dictionary<string, string> {
                { Current.IndexButton.Name, "首页" },
                { Current.GirlButton.Name, "美女" },
                {Current.FashionlButton.Name, "时尚" },
                {Current.PalythingButton.Name, "玩物" },
                {Current.EntButton.Name, "娱乐" },
            };

        }

        #region Properties and state
        public static ImagePage Current;
        #endregion

    }
}
