using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Entities
{
    public class Bill
    {
        [Key]
        public int BillID { get; set; }
        public string Username { get; set; }
        public double SubTotal { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastTimeChange { get; set; }
        public string BillStatusCode { get; set; }
    }
}
