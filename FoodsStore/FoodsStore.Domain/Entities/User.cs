using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Entities {
    public class User {

        [Key]
        [Required]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "{0} must be from {2} to {1} characters")]
        [RegularExpression(pattern: @"^[a-zA-z0-9]{2,64}", ErrorMessage = "{0} just allow characters: a-z, A-Z and 0-9.")]
        public string Username { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 6, ErrorMessage = "{0} must be from {2} to {1} characters")]
        public string Password { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "{0} must be from {2} to {1} characters")]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string RoleID { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public byte[] ImageData { get; set; }

        public string ImageMimeType { get; set; }
    }
}
