using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ENRZ.Core.Helpers {
    public static class VisiEnumHelper {
        public static Visibility GetVisibility(bool isVisible) { return isVisible ? Visibility.Visible : Visibility.Collapsed; }
    }
}
