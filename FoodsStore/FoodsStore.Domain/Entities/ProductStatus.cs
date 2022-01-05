using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Entities
{
    public class ProductStatus
    {
        [Key]
        public string StatusCode { get; set; }
        public string Status { get; set; }
    }
}
