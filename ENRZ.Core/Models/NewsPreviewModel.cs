using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENRZ.Core.Models {
    public class NewsPreviewModel {
        // model type
        public PreviewType ModelType { get; set; }

        // whole state
        public string Title { get; set; }
        public Uri PathUri { get; set; }
        public Uri ImageUri { get; set; }

        // common type
        public string StampTitle { get; set; }
        public string StampDate { get; set; }
        public Uri StampUri { get; set; }
        public string Description { get; set; }

        // pictures type
        public List<SimpleImgModel> SlideImageList { get; set; }
        public List<SimpleImgModel> TopImageList { get; set; }
        public List<SimpleImgModel> RecommendImageList { get; set; }
        public List<SimpleImgModel> SelectImageList { get; set; }
        public List<SimpleImgModel> GirlImageList { get; set; }
        public List<SimpleImgModel> FashionImageList { get; set; }
        public List<SimpleImgModel> PlaythingImageList { get; set; }
        public List<SimpleImgModel> EntImageList { get; set; }

        public enum PreviewType { Common = 0, Pictures = 1,}
    }
}
