using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Entities
{
    public class Kind
    {
        [Key]
        public int KindID { get; set; }
        public string KindOfProduct { get; set; }
    }
}
