using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Entities
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name cannot empty!")]
        [StringLength(200)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Description cannot empty!")]
        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Quantity cannot empty!")]
        [Range(typeof(int), "0", "1000", ErrorMessage = "Quantity has value from 1 to 1000")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price cannot empty!")]
        [Range(0.1, 100, ErrorMessage = "Price has value from $0.1 to $100")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Category cannot empty!")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Date created cannot empty!")]
        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public string UserModified { get; set; }

        [Required(ErrorMessage = "Status cannot empty!")]
        public string StatusCode { get; set; }

        public byte[] ImageData { get; set; }

        public string ImageMimeType { get; set; }
    }
}
