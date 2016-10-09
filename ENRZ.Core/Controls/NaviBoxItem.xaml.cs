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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ENRZ.Core.Controls {
    public sealed partial class NaviBoxItem : UserControl {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NaviBoxItem), null);
        public string Title {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty PathUriProperty = DependencyProperty.Register("PathUri", typeof(string), typeof(NaviBoxItem), null);
        public string PathUri {
            get { return GetValue(PathUriProperty) as string; }
            set { SetValue(PathUriProperty, value); }
        }

        public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register("IconColor", typeof(SolidColorBrush), typeof(NaviBoxItem), null);
        public SolidColorBrush IconColor {
            get { return GetValue(IconColorProperty) as SolidColorBrush; }
            set { SetValue(IconColorProperty, value); }
        }

        public NaviBoxItem() {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
