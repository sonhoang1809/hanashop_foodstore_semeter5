using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models
{
    public class CartDetailsStatusModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Problem { get; set; }
        public double NewPrice { get; set; }
        public double OldPrice { get; set; }
        public int QuantityInStock { get; set; }
    }
}