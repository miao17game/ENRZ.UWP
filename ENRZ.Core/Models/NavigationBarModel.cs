using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENRZ.Core.Models {
    public class NavigationBarModel {
        public string Title { get; set; }
        public Uri PathUri { get; set; }
        public List<BarItemModel> Items { get; set; }
    }
}
