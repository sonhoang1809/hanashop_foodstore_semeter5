using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodsStore.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string errorMessage)
        {
            //Response.StatusCode = id;
            ViewBag.errorMessage = errorMessage;
            return View();
        }

        public ViewResult InternalServerError() {
            Response.StatusCode = 500;
            return View();
        }

        public ViewResult PageNotFound() {
            Response.StatusCode = 404;
            return View();
        }
    }
}