using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENRZ.Core.Models {
    public class PicturesCollModel {
        public List<BarItemModel> PictureItems { get; set; }
        public List<SimpleImgModel> MoreCollection { get; set; }
        public SimpleImgModel Previous { get; set; }
        public SimpleImgModel Next { get; set; }
    }
}
