using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENRZ.Core.Models {
    public class NewsPreviewModel {
        public string StampTitle { get; set; }
        public string StampDate { get; set; }
        public Uri StampUri { get; set; }
        public string Title { get; set; }
        public Uri PathUri { get; set; }
        public Uri ImageUri { get; set; }
        public string Description { get; set; }
        public string Anotation { get { return StampTitle + "  " + StampDate; } }
    }
}
