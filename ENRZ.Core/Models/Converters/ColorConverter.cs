using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ENRZ.Core.Models.Converters {
    public class ColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToColorSolidBrush(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private Brush ToColorSolidBrush(string title) {
            SolidColorBrush result = new SolidColorBrush();
            switch (title) {
                case "首页": result.Color = Color.FromArgb(255,75,21,173); break;
                case "尤物": result.Color = Color.FromArgb(255, 217, 6, 94); break;
                case "资讯": result.Color = Color.FromArgb(255, 60, 188, 98); break;
                case "恋物癖": result.Color = Color.FromArgb(255, 97, 17, 171); break;
                case "时装": result.Color = Color.FromArgb(255, 254, 183, 8); break;
                case "男性人物": result.Color = Color.FromArgb(255, 69, 90, 172); break;
                case "专题": result.Color = Color.FromArgb(255, 141, 4, 33); break;
                case "美图": result.Color = Color.FromArgb(255, 244, 78, 97); break;
                case "商城": result.Color = Color.FromArgb(255, 53, 132, 154); break;
                default: result.Color = Color.FromArgb(255, 82, 82,82); break;
            }
            return result as Brush;
        }
    }
}
