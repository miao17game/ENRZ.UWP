using ENRZ.Core.Controls;
using ENRZ.Core.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace ENRZ.Core.Tools {
    public static class DataProcess {
        #region Properties and State

        private const string HomeHost = "http://www.enrz.com/";
        private const string HomeHostInsert = "http://www.enrz.com";
        private const string MatchHost = "http://www.dongqiudi.com/match";
        private const string ArticleHost = "http://dongqiudi.com/article/";
        private const string DefaultImageFlagHost = "http://static1.dongqiudi.com/web-new/web/images/defaultTeam.png";
        private const string DefaultMenberHost = "http://static1.dongqiudi.com/web-new/web/images/defaultPlayer.png";
        private enum TableItemType { Round = 0, Away = 1, Home = 2, Link = 3, Vs = 4, Stat = 5, Live = 6 ,Times = 7 }

        #endregion

        public static async void ReportError(string erroeMessage) {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                new ToastSmooth("获取数据发生错误\n"+erroeMessage).Show();
            });
        }

        public static async void ReportException(string erroeMessage) {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                new ToastSmooth(erroeMessage).Show();
            });
        }

        public static Uri ConvertToUri(string str) { return !string.IsNullOrEmpty(str) ? new Uri(str) : null; }

        public static List<NavigationBarModel> FetchNavigationBarFromHtml(string htmlResources) {
            var pageResources = new HtmlDocument();
            pageResources.LoadHtml(htmlResources);
            HtmlNode rootnode = pageResources.DocumentNode;
            string XPathString = "//ul[@id='globalnav']";
            HtmlNodeCollection LiCollection = rootnode.SelectSingleNode(XPathString).SelectNodes("li");
            var list = new List<NavigationBarModel>();
            // Fetch each NavigationBar Item
            foreach (var li in LiCollection) {
                if (li.SelectSingleNode("a") != null) {
                    var naviBarModel = new NavigationBarModel();
                    naviBarModel.Title = li.SelectSingleNode("a").InnerText;
                    naviBarModel.PathUri = new Uri(li.SelectSingleNode("a").Attributes["href"].Value);
                    // Check if it has innerItems
                    if (li.SelectSingleNode("ul[@class='gn-sub']") != null) {
                        var innerList = new List<BarItemModel>();
                        foreach (var innerLi in li.SelectSingleNode("ul[@class='gn-sub']").SelectNodes("li")) {
                            var itemModel = new BarItemModel();
                            itemModel.Title = innerLi.SelectSingleNode("a").InnerText;
                            itemModel.PathUri = new Uri(innerLi.SelectSingleNode("a").Attributes["href"].Value);
                            innerList.Add(itemModel);
                        }
                    }

                    list.Add(naviBarModel);
                }
            }

            return list;
        }

    }
}
