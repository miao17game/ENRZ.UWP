using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace ENRZ.Core.Tools {
    /// <summary>
    /// UWP states Helper
    /// </summary>
    public static class UWPStates {

        public static SystemNavigationManager NavigateManager { get { return SystemNavigationManager.GetForCurrentView(); } }
        public static ApplicationView AppView { get { return ApplicationView.GetForCurrentView(); } }

        /// <summary>
        /// Current window height with NavigationBar and StatusBar
        /// </summary>
        public static double WindowWidth { get { return Window.Current.Bounds.Width; } }
        /// <summary>
        /// Current window width with NavigationBar and StatusBar
        /// </summary>
        public static double WindowHeight { get { return Window.Current.Bounds.Height; } }
        /// <summary>
        /// Current window height without NavigationBar and StatusBar
        /// </summary>
        public static double VisibleHeight { get { return ApplicationView.GetForCurrentView().VisibleBounds.Height; } }
        /// <summary>
        /// Current window width without NavigationBar and StatusBar
        /// </summary>
        public static double VisibleWidth { get { return ApplicationView.GetForCurrentView().VisibleBounds.Width; } }

        public static bool IsMobile { get { return AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"); } }

        public static void SetVisibility(FrameworkElement element, bool IsVisible) { element.Visibility = IsVisible? Visibility.Visible : Visibility.Collapsed; }

    }
}
