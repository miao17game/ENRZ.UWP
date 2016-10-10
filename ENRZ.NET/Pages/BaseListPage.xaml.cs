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
    public sealed partial class BaseListPage : Page {
        public BaseListPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var args = e.Parameter as NavigateParameter;
            Args.Text = args.Items==null?args.PathUri.ToString(): args.PathUri.ToString()+ "----->" +args.Items.Count.ToString();
            HeaderResources.Source = args.Items;
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
            //(GridViewResources.Source as List<NewsPreviewModel>).ForEach(i => { Debug.WriteLine(i.ImageUri); });
        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e) {

        }
    }
}
