using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models
{
    public class GraphDashBoardModel
    {
        public List<int> ListNumberBillEachDay { get; set; }
        public List<double> ListRevenueEachDay { get; set; }
        public List<string> ListDate { get; set; }
        public Hashtable ListQuantityProductIsSoldHasCategory { get; set; }
        public Hashtable ListQuantityProductInStorage { get; set; }
        public List<string> ListKind { get; set; }
        public List<int> ListNumberProductInStorageHasKind { get; set; }
        public List<string> ListCate { get; set; }
        public Hashtable ListQuantityProductInStorageHasCategory { get; set; }
        public int NumberBillToday { get; set; }
        public double RevenueToday { get; set; }
    }
}