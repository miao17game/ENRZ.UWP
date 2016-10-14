using DBCSCodePage;
using ENRZ.Core.Controls;
using ENRZ.Core.Models;
using ENRZ.Core.Models.PageContentModels;
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

using static ENRZ.Core.Tools.UWPStates;

namespace ENRZ.Core.Tools {
    public static class DataProcess {
        #region Properties and State
        public enum BarItemType { NavigationOutside = 0, InnerBarItem = 1,}
        #endregion

        public static async void ReportError(string erroeMessage) {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                new ToastSmooth($"{GetUIString("FetchDataError")}\n"+erroeMessage).Show();
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
                    naviBarModel.Title = ToGlobalization(li.SelectSingleNode("a").InnerText);
                    naviBarModel.PathUri = new Uri(li.SelectSingleNode("a").Attributes["href"].Value);
                    // Check if it has innerItems
                    naviBarModel.Items = new List<BarItemModel>(); 
                    naviBarModel.Items.Add(new BarItemModel { Title = naviBarModel.Title, PathUri = naviBarModel.PathUri, });
                    if (li.SelectSingleNode("ul[@class='gn-sub']") != null) {
                        foreach (var innerLi in li.SelectSingleNode("ul[@class='gn-sub']").SelectNodes("li")) {
                            naviBarModel.Items.Add(new BarItemModel {
                                Title = ToGlobalization(innerLi.SelectSingleNode("a").InnerText),
                                PathUri = new Uri(innerLi.SelectSingleNode("a").Attributes["href"].Value),
                            });
                        }
                    }

                    list.Add(naviBarModel);
                }
            }

            return list;
        }

        private static string ToGlobalization(string input) {
            var result = default(string);
            switch (input) {
                case "首页":
                    result = "ENRZ.COM";
                    break;
                case "尤物":
                    result = GetUIString("Stunner");
                    break;
                case "资讯":
                    result = GetUIString("Information");
                    break;
                case "恋物癖":
                    result = GetUIString("LoveOfHabit");
                    break;
                case "时装":
                    result = GetUIString("Fashion");
                    break;
                case "男性人物":
                    result = GetUIString("MaleCharacters");
                    break;
                case "专题":
                    result = GetUIString("Topics");
                    break;
                case "美图":
                    result = GetUIString("Gallery");
                    break;
                case "商城":
                    result = GetUIString("Mall");
                    break;
                case "封面明星":
                    result = GetUIString("CoverStars");
                    break;
                case "新闻女郎":
                    result = GetUIString("NewsGirl");
                    break;
                case "猎奇":
                    result = GetUIString("See");
                    break;
                case "体育":
                    result = GetUIString("Sports");
                    break;
                case "地球村":
                    result = GetUIString("TheGlobalVillage");
                    break;
                case "艺和团":
                    result = GetUIString("Art");
                    break;
                case "汽车":
                    result = GetUIString("Cars");
                    break;
                case "数码":
                    result = GetUIString("Digital");
                    break;
                case "腕表":
                    result = GetUIString("Watch");
                    break;
                case "美酒":
                    result = GetUIString("Wine");
                    break;
                case "户外":
                    result = GetUIString("Outdoor");
                    break;
                case "搭配栏目":
                    result = GetUIString("Collocation");
                    break;
                case "男装大片":
                    result = GetUIString("Men_large");
                    break;
                case "美容护肤":
                    result = GetUIString("SkinCare");
                    break;
                case "后雅皮":
                    result = GetUIString("Yuppie");
                    break;
                case "男人帮":
                    result = GetUIString("Men");
                    break;
                case "甲方乙方":
                    result = GetUIString("A_B");
                    break;
                case "美女":
                    result = GetUIString("Beauty");
                    break;
                case "性感写真":
                    result = GetUIString("Photographic");
                    break;
                case "香车美女":
                    result = GetUIString("RC");
                    break;
                case "体育宝贝":
                    result = GetUIString("SportsBaby");
                    break;
                case "时尚":
                    result = GetUIString("Fashions");
                    break;
                case "秀场":
                    result = GetUIString("Show");
                    break;
                case "大片":
                    result = GetUIString("Swaths");
                    break;
                case "搭配":
                    result = GetUIString("mix");
                    break;
                case "娱乐":
                    result = GetUIString("Ent");
                    break;
                case "八卦热点":
                    result = GetUIString("HotGossip");
                    break;
                case "热辣吐槽":
                    result = GetUIString("Complain");
                    break;
                case "玩物":
                    result = GetUIString("Plaything");
                    break;
                case "座驾":
                    result = GetUIString("Car");
                    break;
                case "摄影":
                    result = GetUIString("Photography");
                    break;
                default:
                    result = input;
                    break;
            }
            return result;
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
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
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
            for(int i = 0; i < 4; i++) 
                FetchCollectionResources(classC, model, i);
            return model;
        }

        public static List<BarItemModel> FetchBarItemNaviFromHtml(string htmlResources) {
            var pageResources = new HtmlDocument();
            pageResources.LoadHtml(htmlResources);
            HtmlNode rootnode = pageResources.DocumentNode;
            string XPathString = "//div[@class='li_nav']";
            var LiNavigation = rootnode.SelectSingleNode(XPathString);
            if (LiNavigation == null) 
                return null;
            var navi_Li_Coll = LiNavigation 
                .SelectSingleNode("//div[@class='lin_nav_c']")
                .SelectSingleNode("//ul[@id='lin_nav']")
                .SelectNodes("li");
            var list = new List<BarItemModel>();
            foreach (var li in navi_Li_Coll) {
                try {
                    list.Add(new BarItemModel {
                        Title = ToGlobalization(li.SelectSingleNode("a").Attributes["title"].Value),
                        PathUri = new Uri(li.SelectSingleNode("a").Attributes["href"].Value),
                    });
                } catch (NullReferenceException ex) {
                    Debug.WriteLine(ex.StackTrace);
                } catch (ArgumentNullException ex) {
                    Debug.WriteLine(ex.StackTrace);
                } catch (UriFormatException ex) {
                    Debug.WriteLine(ex.StackTrace);
                } catch (Exception ex) {
                    Debug.WriteLine(ex.StackTrace);
                }
            }
            return list;
        }

        public static List<SimpleImgModel> FetchBarItemInnerFromHtml(string htmlResources) {
            var pageResources = new HtmlDocument();
            pageResources.LoadHtml(htmlResources);
            HtmlNode rootnode = pageResources.DocumentNode;
            try {
                string XPathString = "//div[@class='li_c fn-clear']";
                var navi_Li_Coll = rootnode
                    .SelectSingleNode(XPathString)
                    .SelectSingleNode("//div[@class='li_l']")
                    .SelectSingleNode("//ul[@class='li_lu li_lu_small']")
                    .SelectNodes("li");
                var list = new List<SimpleImgModel>();
                foreach (var li in navi_Li_Coll) {
                    try {
                        list.Add(new SimpleImgModel {
                            Title = li.SelectSingleNode("a").Attributes["title"].Value,
                            PathUri = new Uri(li.SelectSingleNode("a").Attributes["href"].Value),
                            ImageUri = new Uri(li.SelectSingleNode("a").SelectSingleNode("img").Attributes["src"].Value),
                        });
                    } catch (NullReferenceException ex) {
                        Debug.WriteLine(ex.StackTrace);
                    } catch (ArgumentNullException ex) {
                        Debug.WriteLine(ex.StackTrace);
                    } catch (UriFormatException ex) {
                        Debug.WriteLine(ex.StackTrace);
                    } catch (Exception ex) {
                        Debug.WriteLine(ex.StackTrace);
                    }
                }
                return list;
            } catch {
                return new List<SimpleImgModel>();
            }
        }

        public static PicturesCollModel FetchPictureCollectionFromHtml(string htmlResources) {
            var addHost = "http://pic.enrz.com";
            var pageResources = new HtmlDocument();
            pageResources.LoadHtml(htmlResources);
            HtmlNode rootnode = pageResources.DocumentNode;
            try {
                string XPathString = "//div[@class='detail_rel']";
                var navi_Li_Coll = rootnode.SelectSingleNode(XPathString).SelectNodes("div");
                var model = new PicturesCollModel();
                model.PictureItems = new List<BarItemModel>();
                foreach (var div in navi_Li_Coll) {
                    try {
                        switch (div.Attributes["id"].Value) {
                            case "hover1": try {
                                    model.Previous = new SimpleImgModel {
                                        Title = div.SelectSingleNode("a").Attributes["title"].Value,
                                        PathUri = new Uri(addHost + div.SelectSingleNode("a").Attributes["href"].Value),
                                        ImageUri = new Uri(div.SelectSingleNode("a").SelectSingleNode("img").Attributes["src"].Value)
                                    };
                                } catch (Exception ex) { Debug.WriteLine(ex.StackTrace); }
                                break;
                            case "hover2": try {
                                    var UlCollection = div
                                  .SelectSingleNode("//div[@class='detail_thumb_cm']")
                                  .SelectNodes("ul");
                                    foreach (var ul in UlCollection) {
                                        foreach(var li in ul.SelectNodes("li")) {
                                            model.PictureItems.Add(new BarItemModel {
                                                PathUri = new Uri(addHost + li.SelectSingleNode("a").Attributes["href"].Value),
                                            });
                                        }
                                    }
                                } catch (Exception ex) { Debug.WriteLine(ex.StackTrace); }
                                break;
                            case "hover3": try {
                                    model.Next = new SimpleImgModel {
                                        Title = div.SelectSingleNode("a").Attributes["title"].Value,
                                        PathUri = new Uri(addHost + div.SelectSingleNode("a").Attributes["href"].Value),
                                        ImageUri = new Uri(div.SelectSingleNode("a").SelectSingleNode("img").Attributes["src"].Value)
                                    };
                                } catch (Exception ex) { Debug.WriteLine(ex.StackTrace); }
                                break;
                            default:break;
                        }
                    } catch (Exception ex) { Debug.WriteLine(ex.StackTrace); }
                }
                model.MoreCollection = new List<SimpleImgModel>();
                try {
                    var moreCollection = rootnode
                 .SelectSingleNode("//div[@class='detail_m fn-clear mt20']")
                 .SelectSingleNode("div[@class='detail_recom']")
                 .SelectSingleNode("ul")
                 .SelectNodes("li");
                    foreach (var li in moreCollection) {
                        var noteA = li.SelectSingleNode("a");
                        model.MoreCollection.Add(new SimpleImgModel {
                            Title = noteA.Attributes["title"].Value,
                            PathUri = new Uri(noteA.Attributes["href"].Value),
                            ImageUri = new Uri(noteA.SelectSingleNode("img").Attributes["src"].Value),
                        });
                    }
                } catch (Exception ex) { Debug.WriteLine(ex.StackTrace); }
                try {
                    var moreCollection2 = rootnode
                   .SelectSingleNode("//div[@class='detail_m fn-clear mt20']")
                   .SelectSingleNode("div[@class='detail_u']")
                   .SelectSingleNode("ul")
                   .SelectNodes("li");
                    foreach (var li in moreCollection2) {
                        var noteA = li.SelectSingleNode("a");
                        model.MoreCollection.Add(new SimpleImgModel {
                            Title = noteA.Attributes["title"].Value,
                            PathUri = new Uri(noteA.Attributes["href"].Value),
                            ImageUri = new Uri(noteA.SelectSingleNode("img").Attributes["src"].Value),
                        });
                    }
                } catch ( Exception ex ) { Debug.WriteLine(ex.StackTrace); }
                return model;
            } catch {
                return new PicturesCollModel();
            }
        }

        public static SimpleImgModel FetchPictureSingleFromHtml(string htmlResources) {
            var pageResources = new HtmlDocument();
            pageResources.LoadHtml(htmlResources);
            HtmlNode rootnode = pageResources.DocumentNode;
            try {
                string XPathString = "//div[@class='detail_view_m']";
                var imaDiv = rootnode.SelectSingleNode(XPathString);
                var model = new SimpleImgModel();
                try {
                    model.ImageUri = new Uri(imaDiv.SelectSingleNode("img").Attributes["src"].Value);
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
                return model;
            } catch {
                return null;
            }
        }

        public static PageContentModel GetPageInnerContent(string htmlResources) {
            var model = new PageContentModel();
            try {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlResources);
                HtmlNode rootnode = doc.DocumentNode;
                string XPathString = "//div[@id='main-contbox']";
                var pageBox = rootnode.SelectSingleNode(XPathString);
                var select01 = pageBox.SelectSingleNode("div[@class='mc-content fl']").SelectSingleNode("div[@id='content']");
                model.Title = select01.SelectSingleNode("h2").InnerText;
                model.PreviewContent = select01.SelectSingleNode("h3").InnerText;
                var contents = select01.SelectNodes("p");
                uint index = 0;
                model.ContentImage = new List<ContentImages>();
                model.ContentString = new List<ContentStrings>();
                model.ContentGif = new List<ContentGifs>();
                model.ContentVideo = new List<ContentVideos>();
                model.ContentFlash = new List<ContentFlashs>();
                model.ContentSelfUri = new List<ContentSelfUris>();
                foreach (var item in contents) {
                    try {
                        index++;
                        if (item.SelectSingleNode("a") != null) {
                            if (item.SelectSingleNode("a").SelectSingleNode("img") != null) {
                                if (item.SelectSingleNode("script") == null) {
                                    model.ContentImage.Add(new ContentImages {
                                        ImageSource = new Uri(item.SelectSingleNode("a").SelectSingleNode("img").Attributes["src"].Value),
                                        Index = index });
                                }
                            }
                        } else if (item.SelectSingleNode("script") != null) {
                            // do nothing
                        } else {
                            model.ContentString.Add(new ContentStrings { Content = item.InnerText, Index = index });
                        }
                    } catch (NullReferenceException) {
                        ReportException(GetUIString("ValidToShow"));
                    } catch (ArgumentOutOfRangeException ex) {
                        Debug.WriteLine(ex.StackTrace);
                    } catch (ArgumentNullException ex) {
                        Debug.WriteLine(ex.StackTrace);
                    } catch (FormatException ex) {
                        Debug.WriteLine(ex.StackTrace);
                    } catch (Exception ex) {
                        Debug.WriteLine(ex.StackTrace);
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.StackTrace);
            }
            return model;
        }

        #region Inner Methods

        private static void FetchSlideResources(HtmlNode classC, NewsPreviewModel model) {
            var classC1 = classC.SelectSingleNode("div[@class='c1']");
            if (classC1 != null) {
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
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                } catch { ReportError("Slider source fetch error."); }
            }
        }

        private static void FetchTopResources(HtmlNode classC, NewsPreviewModel model) {
            var classC1 = classC.SelectSingleNode("div[@class='c1']");
            if (classC1 != null) {
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
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                } catch { ReportError("Top5 source fetch error."); }
            }
        }

        private static void FetchRecommendResources(HtmlNode classC, NewsPreviewModel model) {
            var classC2 = classC.SelectSingleNode("div[@class='c2']");
            if (classC2 != null) {
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
                                Debug.WriteLine(ex.StackTrace);
                            } catch (ArgumentNullException ex) {
                                Debug.WriteLine(ex.StackTrace);
                            } catch (UriFormatException ex) {
                                Debug.WriteLine(ex.StackTrace);
                            } catch (Exception ex) {
                                Debug.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                } catch { ReportError("Recommend source fetch error."); }
            }
        }

        private static void FetchSelectResources(HtmlNode classC, NewsPreviewModel model) {
            var classC2 = classC.SelectSingleNode("div[@class='c2']");
            if (classC2 != null) {
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
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
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
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                } catch { ReportError("Select source fetch error."); }
            }
        }

        private static void FetchCollectionResources(HtmlNode classC, NewsPreviewModel model, int index) {
            var classC3 = classC.SelectNodes("div[@class='c3']").ElementAt(index);
            var targetList = new List<SimpleImgModel>();
            if (classC3 != null) {
                try {
                    var classC1_Left = classC3.SelectNodes("div[@class='c3_l']");
                    foreach (var item in classC1_Left) {
                        try {
                            var aRoute = item.SelectSingleNode("a");
                            targetList.Add(new SimpleImgModel {
                                Title = aRoute.Attributes["title"].Value,
                                PathUri = new Uri(aRoute.Attributes["href"].Value),
                                ImageUri = new Uri(aRoute.SelectSingleNode("img").Attributes["src"].Value),
                            });
                        } catch (NullReferenceException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                    var classC1_Li = classC3
                        .SelectSingleNode("div[@class='c3_r']")
                        .SelectSingleNode("ul")
                        .SelectNodes("li");
                    foreach (var item in classC1_Li) {
                        try {
                            var aRoute = item.SelectSingleNode("a");
                            targetList.Add(new SimpleImgModel {
                                Title = aRoute.Attributes["title"].Value,
                                PathUri = new Uri(aRoute.Attributes["href"].Value),
                                ImageUri = new Uri(aRoute.SelectSingleNode("img").Attributes["src"].Value),
                            });
                        } catch (NullReferenceException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (ArgumentNullException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (UriFormatException ex) {
                            Debug.WriteLine(ex.StackTrace);
                        } catch (Exception ex) {
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                } catch { ReportError("Collection source fetch error."); }
                if (index == 1) {
                    var classC3B = classC.SelectSingleNode("div[@class='c3_b']");
                    if (classC3B != null) {
                        try {
                            var classSpecial = classC3B
                                .SelectNodes("div");
                            foreach (var item in classSpecial) {
                                try {
                                    var aRoute = item.SelectSingleNode("a");
                                    targetList.Add(new SimpleImgModel {
                                        Title = aRoute.Attributes["title"].Value,
                                        PathUri = new Uri(aRoute.Attributes["href"].Value),
                                        ImageUri = new Uri(aRoute.SelectSingleNode("img").Attributes["src"].Value),
                                    });
                                } catch (NullReferenceException ex) {
                                    Debug.WriteLine(ex.StackTrace);
                                } catch (ArgumentNullException ex) {
                                    Debug.WriteLine(ex.StackTrace);
                                } catch (UriFormatException ex) {
                                    Debug.WriteLine(ex.StackTrace);
                                } catch (Exception ex) {
                                    Debug.WriteLine(ex.StackTrace);
                                }
                            }
                        } catch { ReportError("Special source fetch error."); }
                    }
                }
                switch (index) {
                    case 0: model.GirlImageList = targetList; break;
                    case 1: model.FashionImageList = targetList; break;
                    case 2: model.PlaythingImageList = targetList; break;
                    case 3: model.EntImageList = targetList; break;
                    default: break;
                }
            }
        }

        #endregion

    }
}
