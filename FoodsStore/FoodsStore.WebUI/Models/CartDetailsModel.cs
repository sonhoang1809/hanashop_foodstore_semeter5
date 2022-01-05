using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models
{
    public class CartDetailsModel
    {
        public int BillID { get; set; } 
        public List<ProductInCartDetailsModel> ProductsInCart { get; set; }
        public double SubTotal { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public DateTime DateCreated { get; set; }
        public string LastTimeChange { get; set; }
        public int QuantityProduct { get; set; }
        
    }
}