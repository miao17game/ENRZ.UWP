using DBCSCodePage;
using ENRZ.Core.Controls;
using ENRZ.Core.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace ENRZ.Core.Tools {
    public static class DataProcess {
        #region Properties and State

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
                    naviBarModel.Items = new List<BarItemModel>();
                    naviBarModel.Items.Add(new BarItemModel { Title = "首页", PathUri = naviBarModel.PathUri, });
                    if (li.SelectSingleNode("ul[@class='gn-sub']") != null) {
                        foreach (var innerLi in li.SelectSingleNode("ul[@class='gn-sub']").SelectNodes("li")) {
                            naviBarModel.Items.Add(new BarItemModel {
                                Title = innerLi.SelectSingleNode("a").InnerText,
                                PathUri = new Uri(innerLi.SelectSingleNode("a").Attributes["href"].Value),
                            });
                        }
                    }

                    list.Add(naviBarModel);
                }
            }

            return list;
        }

        public static List<NewsPreviewModel> FetchNewsPreviewFromHtml(string htmlResources) {
            var pageResources = new HtmlDocument();
            pageResources.LoadHtml(htmlResources);
            HtmlNode rootnode = pageResources.DocumentNode;
            string XPathString = "//div[@id='main-contbox']";
            var mainContBox = rootnode.SelectSingleNode(XPathString);
            if (mainContBox == null) 
                return null;
            var FlDivCollection = mainContBox.SelectNodes("div[@class='mc-content fl']");
            var list = new List<NewsPreviewModel>();
            if (FlDivCollection == null) { 
                return null;
            } else { // Primary page model
                foreach (var fl in FlDivCollection) {
                    var SecCollection = fl.SelectNodes("section[@class='list-thumbnail']");
                    foreach (var section in SecCollection) {
                        try {
                            var stampMess = section.SelectSingleNode("div[@class='list-thumbnail-stamp']");
                            list.Add(new NewsPreviewModel {
                                StampTitle = stampMess.SelectSingleNode("a").InnerText,
                                StampUri = new Uri(stampMess.SelectSingleNode("a").Attributes["href"].Value),
                                StampDate = stampMess.InnerText,
                                Title = section.SelectSingleNode("h4").SelectSingleNode("a").InnerText,
                                PathUri = new Uri(section.SelectSingleNode("h4").SelectSingleNode("a").Attributes["href"].Value),
                                ImageUri = new Uri(section.SelectSingleNode("div[@class='list-thumbnail-pic']").SelectSingleNode("a").SelectSingleNode("img").Attributes["src"].Value),
                                Description = section.SelectSingleNode("div[@class='list-thumbnail-desc']").SelectSingleNode("p").InnerText
                            });
                        } catch (NullReferenceException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
            return list;
        }

        public static NewsPreviewModel FetchImagesIndexFromHtml(string htmlResources) {
            var pageResources = new HtmlDocument();
            pageResources.LoadHtml(htmlResources);
            HtmlNode rootnode = pageResources.DocumentNode;
            string XPathString = "//div[@class='c']";
            var classC = rootnode.SelectSingleNode(XPathString);
            if (classC == null) // Pictures page mode failed
                return null;
            var model = new NewsPreviewModel();
            FetchSlideResources(classC, model);
            FetchTopResources(classC, model);
            FetchRecommendResources(classC, model);
            FetchSelectResources(classC, model);
            return model;
        }

        #region Inner Methods
        private static void FetchSlideResources(HtmlNode classC, NewsPreviewModel model) {
            var classC1 = classC.SelectSingleNode("div[@class='c1']");
            if (classC != null) {
                try {
                    var classC1_Li = classC1
                        .SelectSingleNode("div[@class='c1_l']")
                        .SelectSingleNode("div[@class='c1_slide']")
                        .SelectSingleNode("ul")
                        .SelectNodes("li");
                    model.SlideImageList = new List<SimpleImgModel>();
                    foreach (var item in classC1_Li) {
                        try {
                            var aRoute = item.SelectSingleNode("a");
                            model.SlideImageList.Add(new SimpleImgModel {
                                Title = aRoute.Attributes["title"].Value,
                                PathUri = new Uri(aRoute.Attributes["href"].Value),
                                ImageUri = new Uri(aRoute.SelectSingleNode("img").Attributes["src"].Value),
                            });
                        } catch (NullReferenceException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                } catch { ReportError("Slider source fetch error."); }
            }
        }

        private static void FetchTopResources(HtmlNode classC, NewsPreviewModel model) {
            var classC1 = classC.SelectSingleNode("div[@class='c1']");
            if (classC != null) {
                try {
                    var classC1_Li = classC1
                        .SelectSingleNode("div[@class='c1_r']")
                        .SelectSingleNode("ol[@class='o']")
                        .SelectNodes("li");
                    model.TopImageList = new List<SimpleImgModel>();
                    foreach (var item in classC1_Li) {
                        try {
                            var aRoute = item.SelectSingleNode("a[@class='o_l_80']");
                            model.TopImageList.Add(new SimpleImgModel {
                                Title = aRoute.Attributes["title"].Value,
                                PathUri = new Uri(aRoute.Attributes["href"].Value),
                                ImageUri = new Uri(aRoute.SelectSingleNode("img").Attributes["src"].Value),
                            });
                        } catch (NullReferenceException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                } catch { ReportError("Top5 source fetch error."); }
            }
        }

        private static void FetchRecommendResources(HtmlNode classC, NewsPreviewModel model) {
            var classC2 = classC.SelectSingleNode("div[@class='c2']");
            if (classC != null) {
                try {
                    var classC1_Ul = classC2
                        .SelectSingleNode("div[@class='c2_left']")
                        .SelectSingleNode("div[@class='c2_slide']")
                        .SelectSingleNode("div[@class='c2_slide_c']")
                        .SelectNodes("ul[@class='c2_lc']");
                    model.RecommendImageList = new List<SimpleImgModel>();
                    foreach (var ul in classC1_Ul) {
                        var classC1_Li = ul.SelectNodes("li");
                        foreach (var item in classC1_Li) {
                            try {
                                var aRoute = item.SelectSingleNode("a");
                                model.RecommendImageList.Add(new SimpleImgModel {
                                    Title = aRoute.Attributes["title"].Value,
                                    PathUri = new Uri(aRoute.Attributes["href"].Value),
                                    ImageUri = new Uri(aRoute.SelectSingleNode("img").Attributes["src"].Value),
                                });
                            } catch (NullReferenceException ex) {
                                ReportError(ex.Message);
                                Debug.WriteLine(ex.StackTrace);
                            } catch (ArgumentNullException ex) {
                                ReportError(ex.Message);
                                Debug.WriteLine(ex.StackTrace);
                            } catch (UriFormatException ex) {
                                ReportError(ex.Message);
                                Debug.WriteLine(ex.StackTrace);
                            } catch (Exception ex) {
                                ReportError(ex.Message);
                                Debug.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                } catch { ReportError("Recommend source fetch error."); }
            }
        }

        private static void FetchSelectResources(HtmlNode classC, NewsPreviewModel model) {
            var classC2 = classC.SelectSingleNode("div[@class='c2']");
            if (classC != null) {
                try {
                    var classC1_Li = classC2
                        .SelectSingleNode("div[@class='c1_r c2_r']")
                        .SelectSingleNode("div[@class='c2_rc']")
                        .SelectSingleNode("ul[@class='c2_rc_1']")
                        .SelectNodes("li");
                    model.SelectImageList = new List<SimpleImgModel>();
                    foreach (var item in classC1_Li) {
                        try {
                            var aRoute = item.SelectSingleNode("a");
                            model.SelectImageList.Add(new SimpleImgModel {
                                Title = aRoute.Attributes["title"].Value,
                                PathUri = new Uri(aRoute.Attributes["href"].Value),
                                ImageUri = new Uri(aRoute.SelectSingleNode("img").Attributes["src"].Value),
                            });
                        } catch (NullReferenceException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                    var classC1_Div = classC2
                        .SelectSingleNode("div[@class='c1_r c2_r']")
                        .SelectSingleNode("div[@class='c2_rc']")
                        .SelectNodes("div[@class='c2_rc_2']");
                    foreach (var item in classC1_Div) {
                        try {
                            var aRoute = item.SelectSingleNode("a");
                            model.SelectImageList.Add(new SimpleImgModel {
                                Title = aRoute.Attributes["title"].Value,
                                PathUri = new Uri(aRoute.Attributes["href"].Value),
                                ImageUri = new Uri(aRoute.SelectSingleNode("img").Attributes["src"].Value),
                            });
                        } catch (NullReferenceException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            ReportError(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                } catch { ReportError("Select source fetch error."); }
            }
        }

        #endregion

    }
}
