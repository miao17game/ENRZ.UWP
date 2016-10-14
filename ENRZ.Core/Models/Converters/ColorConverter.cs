using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

using static ENRZ.Core.Tools.UWPStates;

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
            result.Color = title == "ENRZ.COM" ? Color.FromArgb(255, 75, 21, 173) :
                title == GetUIString("Stunner") ? Color.FromArgb(255, 217, 6, 94) :
                title == GetUIString("Information") ? Color.FromArgb(255, 60, 188, 98) :
                title == GetUIString("LoveOfHabit") ? Color.FromArgb(255, 97, 17, 171) :
                title == GetUIString("Fashion") ? Color.FromArgb(255, 254, 183, 8) :
                title == GetUIString("MaleCharacters") ? Color.FromArgb(255, 69, 90, 172) :
                title == GetUIString("Topics") ? Color.FromArgb(255, 141, 4, 33) :
                title == GetUIString("Gallery") ? Color.FromArgb(255, 244, 78, 97) :
                title == GetUIString("Mall") ? Color.FromArgb(255, 53, 132, 154) :
                Color.FromArgb(255, 82, 82, 82);
            return result;
        }
    }
}
