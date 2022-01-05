using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models {

    public class ProductListViewModel {

        public IEnumerable<Product> Products { get; set; }

        public int NumProduct { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public IEnumerable<Kind> Kinds { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public SearchInfo SearchInfo { get; set; }
    }
}