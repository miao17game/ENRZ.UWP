using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;

namespace ENRZ.Core.Models.PageContentModels {
    public class ContentStrings {
        public uint Index { get; set; }
        public string Content { get; set; }
        public Color Color { get; set; }
        public TextLineBounds Weight { get; set; }
    }
}
