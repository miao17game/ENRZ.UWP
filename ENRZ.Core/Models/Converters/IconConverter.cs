using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

using static ENRZ.Core.Tools.UWPStates;

namespace ENRZ.Core.Models.Converters {
    public class IconConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToIconCode(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private string ToIconCode(string title) {
            return title == "ENRZ.COM" ? char.ConvertFromUtf32(0xE10F) :
                title == GetUIString("Stunner") ? char.ConvertFromUtf32(0xE95E) :
                title == GetUIString("Information") ? char.ConvertFromUtf32(0xE1CB) :
                title == GetUIString("LoveOfHabit") ? char.ConvertFromUtf32(0xEE57) :
                title == GetUIString("Fashion") ? char.ConvertFromUtf32(0xECA7) :
                title == GetUIString("MaleCharacters") ? char.ConvertFromUtf32(0xEB68) :
                title == GetUIString("Topics") ? char.ConvertFromUtf32(0xECE9) :
                title == GetUIString("Gallery") ? char.ConvertFromUtf32(0xE052) :
                title == GetUIString("Mall") ? char.ConvertFromUtf32(0xE14D) :
                char.ConvertFromUtf32(0xE1F6);
        }
    }
}
