using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENRZ.Core.Models {
    public class SimpleImgModel {
        public string Title { get; set; }
        public Uri PathUri { get; set; }
        public Uri ImageUri { get; set; }
        public uint Index { get; set; }
    }
}
