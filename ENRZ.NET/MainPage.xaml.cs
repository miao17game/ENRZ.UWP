#region Using
using ENRZ.Core.Controls;
using ENRZ.Core.Helpers;
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
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

using static ENRZ.Core.Tools.UWPStates;
using static ENRZ.NET.MainPage.InnerResources;
#endregion

namespace ENRZ.NET {
    
    public sealed partial class MainPage : Page {

        #region Constructor
        public MainPage() {
            this.InitializeComponent();
            Current = this;
            baseListRing.IsActive = true;
            PrepareFrame.Navigate(typeof(PreparePage));
            var isDarkOrNot = (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsDarkThemeOrNot) ?? true;
            RequestedTheme = isDarkOrNot ? ElementTheme.Dark : ElementTheme.Light;
            NavigateManager.BackRequested += OnBackRequested;
            MainContentFrame = this.ContentFrame;
            BaseListRing = this.baseListRing;
            NavigateTitlePath = this.navigateTitlePath;
            ChangeTitlePath(NaviPathTitle.RoutePath);
            StatusBarInit.InitInnerDesktopStatusBar(true);
            Window.Current.SetTitleBar(BasePartBorder);
            IfNeedAdapteVitualNavigationBar();
            InitSlideRecState();
            GetResources();
        }

        #endregion

        #region Events
        private void NavigationSplit_PaneClosed(SplitView sender, object args) {
            SetVisibility(SlideAnimaRec,true);
            OutBorder.Completed += OnOutBorderOut;
            OutBorder.Begin();
        }

        private void OnOutBorderOut(object sender, object e) {
            OutBorder.Completed -= OnOutBorderOut;
            SetVisibility(DarkDivideBorder, false);
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e) {
            if (ContentFrame.Content == null) {
                if (!isNeedClose) { InitCloseAppTask(); } else { Application.Current.Exit(); }
                e.Handled = true;
                return;
            } else 
                ContentFrame.Content = null;
            e.Handled = true;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            OpenOrClosePane();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            BasePartFrame.Navigate(typeof(SettingsPage));
            HamburgerListBox.SelectedIndex = -1;
        }

        private void HamburgerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            baseListRing.IsActive = false;
            NavigationSplit.IsPaneOpen = false;
            var model = e.AddedItems.FirstOrDefault() as NavigationBarModel;
            if (model == null)
                return;
            ChangeTitlePath(2, (sender as ListBox).SelectedIndex == 0 ? null : model.Title);
            NavigateType type = model.Title.ToString() == GetUIString("Gallery") ?
            NavigateType.ImageNaviBar :
            NavigateType.NaviBar;
            if (IfContainsPageInstance(NaviPathTitle.RoutePath)) {
                GetFrameInstance(type).Content = GetPageInstance(NaviPathTitle.RoutePath);
            } else {
                NavigateToBase?.Invoke(
                    sender,
                    new NavigateParameter { PathUri = model.PathUri, Items = model.Items, },
                    GetFrameInstance(type),
                    GetPageType(type));
            }
        }

        private void ImageControlButton_Click(object sender, RoutedEventArgs e) {
            ImagePopup.IsOpen = false;
        }

        private void ImagePopup_SizeChanged(object sender, SizeChangedEventArgs e) {
            ImagePopupBorder.Width = (sender as Popup).ActualWidth;
            ImagePopupBorder.Height = (sender as Popup).ActualHeight;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            ImagePopup.Width = (sender as Grid).ActualWidth;
            ImagePopup.Height = (sender as Grid).ActualHeight;
        }

        private async void ImageSaveButton_Click(object sender, RoutedEventArgs e) {
            SoftwareBitmap sb = await DownloadImage(PopupImageUri.ToString());
            if (sb != null) {
                SoftwareBitmapSource source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(sb);
                var name = await WriteToFile(sb);
                new ToastSmooth(GetUIString("SaveImageSuccess")).Show();
            }
        }

        #endregion

        #region Methods

        private void InitCloseAppTask() {
            isNeedClose = true;
            new ToastSmooth(GetUIString("ClickTwiceToExit")).Show();
            Task.Run(async () => {
                await Task.Delay(2000);
                isNeedClose = false;
            });
        }

        private void IfNeedAdapteVitualNavigationBar() {
            if (IsMobile) {
                AppView.VisibleBoundsChanged += (s, e) => { AdapteVitualNavigationBarWithoutStatusBar(this); };
                AdapteVitualNavigationBarWithoutStatusBar(this);
            }
        }

        public static void DivideWindowRange(Page currentFramePage, double rangeNum, uint divideNum) {
            if (IsMobile) {
                currentFramePage.Width = VisibleWidth;
                Current.Frame.SizeChanged += (sender, args) => { currentFramePage.Width = VisibleWidth; };
            } else {
                var nowWidth = VisibleWidth;
                currentFramePage.Width = nowWidth > rangeNum ? nowWidth / divideNum : nowWidth;
                Current.Frame.SizeChanged += (sender, args) => {
                    var nowWidthEx = VisibleWidth;
                    currentFramePage.Width = nowWidthEx > rangeNum ? nowWidthEx / divideNum : nowWidthEx;
                };
            }
        }

        private async void GetResources() {
            var list = DataProcess.FetchNavigationBarFromHtml((await WebProcess.GetHtmlResources(HomeHost, false)).ToString());
            NaviBarResouces.Source = list;
        }

        public static void ChangeTitlePath(string value) {
            Current.NavigateTitlePath.Text = value;
        }

        public static void ChangeTitlePath(uint number , string value) {
            if (number < 1 || number > 3)
                return;
            switch (number) {
                case 1: NaviPathTitle.Route01 = value; break;
                case 2: NaviPathTitle.Route02 = value; break;
                case 3: NaviPathTitle.Route03 = value; break;
            }
            Current.NavigateTitlePath.Text = NaviPathTitle.RoutePath;
        }

        private void OnPaneIsOpened() {
            SetVisibility(DarkDivideBorder, true);
            EnterBorder.Begin();
        }

        public static void ShowImageInScreen(Uri imageSource) {
            Current.PopupImageUri = imageSource;
            Current.ImageScreen.Source = new BitmapImage( imageSource);
            Current.ImagePopup.IsOpen = true;
        }

        private async Task<SoftwareBitmap> DownloadImage(string url) {
            try {
                HttpClient hc = new HttpClient();
                HttpResponseMessage resp = await hc.GetAsync(new Uri(url));
                resp.EnsureSuccessStatusCode();
                IInputStream inputStream = await resp.Content.ReadAsInputStreamAsync();
                IRandomAccessStream memStream = new InMemoryRandomAccessStream();
                await RandomAccessStream.CopyAsync(inputStream, memStream);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(memStream);
                SoftwareBitmap softBmp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                return softBmp;
            } catch (Exception) {
                return null;
            }
        }

        public async Task<string> WriteToFile(SoftwareBitmap softwareBitmap) {
            string fileName = "ENRZ" + 
                Guid.NewGuid().GetHashCode().ToString() + "FR" +
                DateTime.Now.Year.ToString() + "GR" +
                DateTime.Now.Month.ToString() + "VE" +
                DateTime.Now.Day.ToString() + "JU" +
                DateTime.Now.Hour.ToString() + "SW" +
                DateTime.Now.Minute.ToString() + "VJ" +
                DateTime.Now.Second.ToString() + "MQ" +
                DateTime.Now.Millisecond.ToString() + ".jpg";

            if (softwareBitmap != null) {
                // save image file to cache
                StorageFile file = await (
                    await KnownFolders.PicturesLibrary.CreateFolderAsync("ENRZ", CreationCollisionOption.OpenIfExists))
                    .CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();
                }
            }

            return fileName;
        }

        #endregion

        #region Inner Resources class
        /// <summary>
        /// Resources Helper
        /// </summary>
        public static class InnerResources{

            public static Type GetPageType(NavigateType type) { return PagesMaps.ContainsKey(type) ? PagesMaps[type] : null; }
            static private Dictionary<NavigateType, Type> PagesMaps = new Dictionary<NavigateType, Type> {
            {NavigateType.NaviBar,typeof(BaseListPage)},
            {NavigateType.Content,typeof(ContentPage)},
            {NavigateType.ImageNaviBar,typeof(ImagePage)},
            {NavigateType.PicutreContent,typeof(PicturesPage)},
            {NavigateType.Settings,typeof(SettingsPage)},
        };

            public static Frame GetFrameInstance(NavigateType type) { return FrameMaps.ContainsKey(type) ? FrameMaps[type] : null; }
            static private Dictionary<NavigateType, Frame> FrameMaps = new Dictionary<NavigateType, Frame> {
            {NavigateType.NaviBar,Current.BasePartFrame},
            {NavigateType.ImageNaviBar,Current.BasePartFrame},
            {NavigateType.Settings,Current.ContentFrame},
            {NavigateType.Content,Current.ContentFrame},
            {NavigateType.PicutreContent,Current.ContentFrame},
        };

            public static void AddBaseListPageInstance(string key, BaseListPage instance) { if (!BaseListPageMap.ContainsKey(key)) { BaseListPageMap.Add(key, instance); } }
            public static BaseListPage GetPageInstance(string key) { return BaseListPageMap.ContainsKey(key) ? BaseListPageMap[key] : null; }
            public static bool IfContainsPageInstance(string key) { return BaseListPageMap.ContainsKey(key); }
            static private Dictionary<string, BaseListPage> BaseListPageMap = new Dictionary<string, BaseListPage> {
            };

        }
        #endregion

        #region Slide Animation Events

        private Storyboard slideStory;
        private TranslateTransform slideTranslateT;

        private void InitSlideRecState() {
            SlideAnimaRec.ManipulationMode = ManipulationModes.TranslateX;
            SlideAnimaRec.ManipulationCompleted += SlideAnimaRec_ManipulationCompleted;
            SlideAnimaRec.ManipulationDelta += SlideAnimaRec_ManipulationDelta;
            slideTranslateT = SlideAnimaRec.RenderTransform as TranslateTransform;
            if (slideTranslateT == null)
                SlideAnimaRec.RenderTransform = slideTranslateT = new TranslateTransform();
        }

        private void SlideAnimaRec_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs args) {
            if (slideTranslateT.X + args.Delta.Translation.X < 0) {
                slideTranslateT.X = 0;
                return;
            }
            slideTranslateT.X += args.Delta.Translation.X;
        }

        private void SlideAnimaRec_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            double abs_delta = Math.Abs(e.Cumulative.Translation.X);
            double speed = Math.Abs(e.Velocities.Linear.X);
            double delta = e.Cumulative.Translation.X;
            double to = 0;

            if (abs_delta < SlideAnimaRec.ActualWidth / 2 && speed < 0.7) {
                slideTranslateT.X = 0;
                return;
            }

            if (delta > 0)
                to = SlideAnimaRec.ActualWidth;
            else if (delta < 0)
                return;

            slideStory = new Storyboard();
            var doubleanimation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromMilliseconds(0)), From = slideTranslateT.X, To = 0 };
            doubleanimation.Completed += Doubleanimation_Completed;
            Storyboard.SetTarget(doubleanimation, slideTranslateT);
            Storyboard.SetTargetProperty(doubleanimation, "X");
            slideStory.Children.Add(doubleanimation);
            slideStory.Begin();
        }

        private void Doubleanimation_Completed(object sender, object e) {
            OpenOrClosePane();
        }

        private void OpenOrClosePane() {
            NavigationSplit.IsPaneOpen = !NavigationSplit.IsPaneOpen;
            SetVisibility(SlideAnimaRec, !NavigationSplit.IsPaneOpen);
            if (NavigationSplit.IsPaneOpen && UWPStates.WindowWidth<800) { OnPaneIsOpened(); }
        }

        #endregion

        #region Properties and state
        public static MainPage Current;
        public TextBlock NavigateTitlePath;
        public Frame MainContentFrame;
        public ProgressRing BaseListRing;
        private bool isNeedClose = false;
        private Uri PopupImageUri;
        public delegate void NavigationEventHandler(object sender, NavigateParameter parameter, Frame frame, Type type);
        public NavigationEventHandler NavigateToBase = (sender, parameter, frame, type) => { frame.Navigate(type, parameter); };
        public struct PathTitle {
            public string Route01 { get; set; }
            public string Route02 { get; set; }
            public string Route03 { get; set; }
            public string RoutePath { get { return Route02!=null?  Route03 != null ? Route01 + "-" + Route02 + "-" + Route03 : Route01 + "-" + Route02 : Route01; } } }
        public static PathTitle NaviPathTitle = new PathTitle { Route01 = "ENRZ.COM" };
        private const string HomeHost = "http://www.enrz.com/";
        private const string HomeHostInsert = "http://www.enrz.com";
        #endregion

    }
}
