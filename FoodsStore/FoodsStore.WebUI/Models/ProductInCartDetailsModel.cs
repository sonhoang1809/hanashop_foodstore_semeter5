
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models
{
    public class ProductInCartDetailsModel
    {
    //    public string IdBill { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Kind { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public string ShortDescription { get; set; }
        public string StatusCode { get; set; }
        
    }
}