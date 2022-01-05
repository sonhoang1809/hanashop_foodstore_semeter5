using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace FoodsStore.WebUI.Support
{
    public class Support
    {
        public static string GetResponseFromUrlFacebook(string url)
        {
            WebRequest wr = WebRequest.Create(url);
            WebResponse ws = wr.GetResponse();
            Stream st = ws.GetResponseStream();
            StreamReader sr = new StreamReader(st);
            string str = sr.ReadToEnd();
            return str;
        }
    }
}