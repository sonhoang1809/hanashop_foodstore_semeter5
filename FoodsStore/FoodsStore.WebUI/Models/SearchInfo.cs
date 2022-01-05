using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models {
    public class SearchInfo {
        public int? KindID { get; set; }
        public int? CategoryID { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public string ProductName { get; set; }

        public SearchInfo() {
        }
    }
}