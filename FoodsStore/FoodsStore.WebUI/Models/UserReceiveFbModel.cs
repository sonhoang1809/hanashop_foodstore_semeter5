using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models
{
    public class UserReceiveFbModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Picture Picture { get; set; }

    }
    public class Picture
    {
        public Data Data { get; set; }
    }
    public class Data
    {
        public string Url { get; set; }
    }
}