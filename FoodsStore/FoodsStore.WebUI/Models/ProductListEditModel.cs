using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models {
    public class ProductListEditModel {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Kind> Kinds { get; set; }
        public int? SelectedKind { get; set; }
        public int? SelectedCategory { get; set; }
        
    }
}