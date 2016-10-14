using ENRZ.Core.Models;
using ENRZ.Core.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using static ENRZ.Core.Tools.UWPStates;

namespace ENRZ.NET.Pages {
   
    public sealed partial class PicturesPage : Page {
        public PicturesPage() {
            this.InitializeComponent();
            MainPage.DivideWindowRange(this, 800, 2);
        }

        #region Methods
        private async System.Threading.Tasks.Task SetPicturesResources(PicturesCollModel source) {
            foreach (var item in source.PictureItems) {
                var grid = new Grid();
                var uri = DataProcess.FetchPictureSingleFromHtml((await WebProcess.GetHtmlResources(item.PathUri.ToString(), true)).ToString()).ImageUri;
                var image = new Image {
                    Source = new BitmapImage(uri),
                    Margin = new Thickness(10, 5, 10, 5),
                    Stretch = Stretch.UniformToFill,
                };
                grid.Children.Add(image);
                var button = new Button {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = new SolidColorBrush(Colors.Transparent),
                    Style = Application.Current.Resources["MainPageButtonBackHamburgerStyle"] as Style,
                };
                button.Click += (sender, clickArgs) => { MainPage.ShowImageInScreen(uri); };
                grid.Children.Add(button);
                ContentStack.Children.Add(grid);
            }
        }

        private void SetPreAndNextResources(PicturesCollModel source) {
            image01.Source = new BitmapImage(source.Previous.ImageUri);
            image02.Source = new BitmapImage(source.Next.ImageUri);
            image01Text.Text = source.Previous.Title;
            image02Text.Text = source.Next.Title;
            previousButton.Click += (sender, clickpre) => {
                MainPage.Current.NavigateToBase?.Invoke(
                    sender,
                    new NavigateParameter { PathUri = source.Previous.PathUri },
                    MainPage.InnerResources.GetFrameInstance(NavigateType.PicutreContent),
                    MainPage.InnerResources.GetPageType(NavigateType.PicutreContent));
            };
            nextButton.Click += (sender, clickpre) => {
                MainPage.Current.NavigateToBase?.Invoke(
                    sender,
                    new NavigateParameter { PathUri = source.Next.PathUri },
                    MainPage.InnerResources.GetFrameInstance(NavigateType.PicutreContent),
                    MainPage.InnerResources.GetPageType(NavigateType.PicutreContent));
            };
        }
        #endregion

        #region Events
        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            contentRing.IsActive = true;
            var args = e.Parameter as NavigateParameter;
            if (args == null) {
                contentRing.IsActive = false;
                return;
            }
            var source = DataProcess.FetchPictureCollectionFromHtml(
                    (await WebProcess.GetHtmlResources(
                        args.PathUri.ToString(), true))
                        .ToString());
            SetPreAndNextResources(source);
            await SetPicturesResources(source);
            // Not Support
            //GridViewResources.Source = source.MoreCollection;
            contentRing.IsActive = false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Current.MainContentFrame.Content = null;
        }

        private void AdaptiveGV_ItemClick(object sender, ItemClickEventArgs e) {

        }
        #endregion
    }
}
