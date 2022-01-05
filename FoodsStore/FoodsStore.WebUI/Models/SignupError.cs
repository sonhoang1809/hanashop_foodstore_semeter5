using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models
{
    public class SignupError
    {
        public String ErrorUserID { get; set; }
        public String ErrorEmail { get; set; }
        public String ErrorName { get; set; }
        public String ErrorPassword { get; set; }
        public String ErrorConfirm { get; set; }
        public String ErrorConfirmPassword { get; set; }
    }
}