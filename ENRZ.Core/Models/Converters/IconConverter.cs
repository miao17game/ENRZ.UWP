using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ENRZ.Core.Models.Converters {
    public class IconConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToIconCode(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private string ToIconCode(string title) {
            var result = default(string);
            switch (title) {
                case "ENRZ.COM": result = char.ConvertFromUtf32(0xE10F); break;
                case "尤物": result = char.ConvertFromUtf32(0xE95E); break;
                case "资讯": result = char.ConvertFromUtf32(0xE1CB); break;
                case "恋物癖": result = char.ConvertFromUtf32(0xEE57); break;
                case "时装": result = char.ConvertFromUtf32(0xECA7); break;
                case "男性人物": result = char.ConvertFromUtf32(0xEB68); break;
                case "专题": result = char.ConvertFromUtf32(0xECE9); break;
                case "美图": result = char.ConvertFromUtf32(0xE052); break;
                case "商城": result = char.ConvertFromUtf32(0xE14D); break;
                default: result = char.ConvertFromUtf32(0xE1F6); break;
            }
            return result;
        }
    }
}
