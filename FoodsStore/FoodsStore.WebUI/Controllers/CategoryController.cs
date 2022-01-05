using FoodsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodsStore.WebUI.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryRepository categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo) {
            this.categoryRepo = categoryRepo;
        }

        public JsonResult GetCategories(int kindId) => Json(categoryRepo.Categories.Where(c => c.KindID == kindId), JsonRequestBehavior.AllowGet);
    }
}