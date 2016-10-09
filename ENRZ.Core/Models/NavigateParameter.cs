using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENRZ.Core.Models {
    public class NavigateParameter {
        public Uri PathUri { get; set; }
        public List<BarItemModel> Items { get; set; }
        public object MessageBag { get; set; }
    }
}
