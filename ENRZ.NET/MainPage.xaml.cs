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
using Windows.Foundation;
using Windows.Foundation.Collections;
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
using Windows.UI.Xaml.Navigation;
#endregion

namespace ENRZ.NET {
    
    public sealed partial class MainPage : Page {

        #region Constructor
        public MainPage() {
            this.InitializeComponent();
            Current = this;
            var isDarkOrNot = (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsDarkThemeOrNot) ?? true;
            RequestedTheme = isDarkOrNot ? ElementTheme.Dark : ElementTheme.Light;
            UWPStates.NavigateManager.BackRequested += OnBackRequested;
            MainContentFrame = this.ContentFrame;
            NavigateTitlePath = this.navigateTitlePath;
            ChangeTitlePath(NaviPathTitle.RoutePath);
            PrepareFrame.Navigate(typeof(PreparePage));
            StatusBarInit.InitInnerDesktopStatusBar(true);
            Window.Current.SetTitleBar(BasePartBorder);
            if (UWPStates.IsMobile) {
                UWPStates.AppView.VisibleBoundsChanged += (s, e) => { ChangeViewWhenNavigationBarChanged(); };
                ChangeViewWhenNavigationBarChanged(); }
            InitSlideRecState();
            GetResources();
        }
        #endregion

        #region Events
        private void NavigationSplit_PaneClosed(SplitView sender, object args) {
            UWPStates.SetVisibility(SlideAnimaRec,true);
            OutBorder.Completed += OnOutBorderOut;
            OutBorder.Begin();
        }

        private void OnOutBorderOut(object sender, object e) {
            OutBorder.Completed -= OnOutBorderOut;
            UWPStates.SetVisibility(DarkDivideBorder, false);
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
            NavigationSplit.IsPaneOpen = false;
            var model = e.AddedItems.FirstOrDefault() as NavigationBarModel;
            if (model == null)
                return;
            NaviPathTitle.Route02 = (sender as ListBox).SelectedIndex == 0 ? null: model.Title;
            ChangeTitlePath(NaviPathTitle.RoutePath);
            NavigateType type = model.Title.ToString() == "美图" ?
            NavigateType.ImageNaviBar :
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
        #endregion

        #region Methods

        private void InitCloseAppTask() {
            isNeedClose = true;
            new ToastSmooth("再按一次返回键退出").Show();
            Task.Run(async () => {
                await Task.Delay(2000);
                isNeedClose = false;
            });
        }

        public static void DivideWindowRange(FrameworkElement element, double rangeNum, uint divideNum) {
            if (UWPStates.IsMobile) {
                element.Width = UWPStates.VisibleWidth;
                Current.Frame.SizeChanged += (sender, args) => { element.Width = UWPStates.VisibleWidth; };
            } else {
                var nowWidth = UWPStates.VisibleWidth;
                element.Width = nowWidth > rangeNum ? nowWidth / divideNum : nowWidth;
                Current.Frame.SizeChanged += (sender, args) => {
                    var nowWidthEx = UWPStates.VisibleWidth;
                    element.Width = nowWidthEx > rangeNum ? nowWidthEx / divideNum : nowWidthEx;
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

        private void ChangeViewWhenNavigationBarChanged() {
            Width = UWPStates.VisibleWidth;
            var wholeHeight = UWPStates.WindowHeight;
            var wholeWidth = UWPStates.WindowWidth;
            if (wholeHeight < wholeWidth) {
                Height = UWPStates.VisibleHeight;
                Width = UWPStates.VisibleWidth + 48;
                Margin =
                    Width - wholeWidth > -0.1 ?
                    new Thickness(0, 0, 0, 0) :
                    new Thickness(-24, 0, 0, 0);
                return;
            }
            Height = UWPStates.VisibleHeight + 24;
            Margin =
                Height - wholeHeight > -0.1 ?
                new Thickness(0, 0, 0, 0) :
                new Thickness(0, -48, 0, 0);
        }

        private void OnPaneIsOpened() {
            UWPStates.SetVisibility(DarkDivideBorder, true);
            EnterBorder.Begin();
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
            UWPStates.SetVisibility(SlideAnimaRec, !NavigationSplit.IsPaneOpen);
            if (NavigationSplit.IsPaneOpen && UWPStates.WindowWidth<800) { OnPaneIsOpened(); }
        }

        #endregion

        #region Properties and state
        public static MainPage Current;
        public TextBlock NavigateTitlePath;
        public Frame MainContentFrame;
        private bool isNeedClose = false;
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
