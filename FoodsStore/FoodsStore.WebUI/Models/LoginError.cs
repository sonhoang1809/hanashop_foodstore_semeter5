using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models
{
    public class LoginError
    {
        public String ErrorNotLoginYet { get; set; }
        public String ErrorExistUserID { get; set; }
        public String ErrorExistEmail { get; set; }
        public String ErrorPassword { get; set; }
    }
}