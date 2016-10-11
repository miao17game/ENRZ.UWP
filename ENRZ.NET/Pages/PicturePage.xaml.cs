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

namespace ENRZ.NET.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PicturePage : Page {
        public PicturePage() {
            this.InitializeComponent();
            if (AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile")) {
                this.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;
                MainPage.Current.Frame.SizeChanged += (sender, args) => { this.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width; };
            } else {
                var nowWidth = ApplicationView.GetForCurrentView().VisibleBounds.Width;
                this.Width = nowWidth > 600 ? nowWidth / 2 : nowWidth;
                MainPage.Current.Frame.SizeChanged += (sender, args) => {
                    var nowWidthEx = ApplicationView.GetForCurrentView().VisibleBounds.Width;
                    this.Width = nowWidthEx > 600 ? nowWidthEx / 2 : nowWidthEx;
                };
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            var args = e.Parameter as NavigateParameter;
            if (args == null)
                return;
            var source = DataProcess.FetchPictureCollectionFromHtml(
                    (await WebProcess.GetHtmlResources(
                        args.PathUri.ToString(), true))
                        .ToString());
            image01.Source = new BitmapImage(source.Previous.ImageUri);
            image02.Source = new BitmapImage(source.Next.ImageUri);
            image01Text.Text = source.Previous.Title;
            image02Text.Text = source.Next.Title;
            foreach (var item in source.PictureItems) {
                var grid = new Grid();
                grid.Children.Add(new Image {
                    Source = new BitmapImage(DataProcess.FetchPictureSingleFromHtml((await WebProcess.GetHtmlResources(item.PathUri.ToString(), true)).ToString()).ImageUri),
                    Margin = new Thickness(10, 5, 10, 5),
                    Stretch = Stretch.UniformToFill,
                });
                var button = new Button {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = new SolidColorBrush(Colors.Transparent),
                    Style = Application.Current.Resources["MainPageButtonBackHamburgerStyle"] as Style,
                };
                button.Click += (sender, clickArgs) => { DataProcess.ReportException("开发中"); };
                grid.Children.Add(button);
                ContentStack.Children.Add(grid);
            }
            GridViewResources.Source = source.MoreCollection;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Current.MainContentFrame.Content = null;
        }

        private void AdaptiveGV_ItemClick(object sender, ItemClickEventArgs e) {

        }
    }
}
