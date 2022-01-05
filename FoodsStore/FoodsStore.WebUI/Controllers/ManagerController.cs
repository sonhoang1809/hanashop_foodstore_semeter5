using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//HI
namespace FoodsStore.WebUI.Controllers {
    public class ManagerController : Controller {
        private IProductRepository productRepo;
        private IKindRepository kindRepo;
        private IProductStatusRepository productStatusRepo;
        private ICategoryRepository categoryRepo;

        public ManagerController(IProductRepository productRepo, IKindRepository kindRepo, IProductStatusRepository productStatusRepo, ICategoryRepository categoryRepo) {
            this.productRepo = productRepo;
            this.kindRepo = kindRepo;
            this.productStatusRepo = productStatusRepo;
            this.categoryRepo = categoryRepo;
        }




        /// <summary>
        /// Hiển thị sang Index của để quản lý Product
        /// Code này hơi tệ, rảnh sẽ sửa lại
        /// </summary>
        /// <param name="kindID"></param>
        /// <param name="categoryID"></param>
        /// <returns>Trả về mảng Kinds, Categories, Products</returns>
        public ActionResult ManageProduct(int? kindID, int? categoryID) {

            IEnumerable<Product> products;
            IEnumerable<Category> categories;
            IEnumerable<Kind> kinds;
            int? tempKindID = null;

            if (kindID == null && categoryID == null) {//Trường hợp không nhập kindID cũng không nhập category => Lẫy kind đầu tiên, category đầu tiên.

                tempKindID = kindRepo.Kinds.First().KindID;
                categories = categoryRepo.Categories.Where(c => c.KindID == tempKindID);
                products = productRepo.Products.OrderBy(p => p.ProductName).Where(p => p.CategoryID == categories.First().CategoryID);

                ViewBag.SelectedKind = tempKindID;
                ViewBag.SelectedCategory = categories.First().CategoryID;


            } else {


                if (kindID == null && categoryID != null) {//Lấy Product theo Category / Trường hợp chỉ nhập CategoryID mà không nhập kindID
                    tempKindID = categoryRepo.Categories.FirstOrDefault(c => c.CategoryID == categoryID).KindID;
                    categories = categoryRepo.Categories.Where(c => c.KindID == tempKindID); //Mảng category cùng Kind
                    ViewBag.SelectedKind = tempKindID;
                } else {
                    categories = categoryRepo.Categories.Where(c => c.KindID == kindID);
                    ViewBag.SelectedKind = kindID;
                }


                if (kindID != null && categoryID == null) {//Nếu chỉ nhập KindID, không nhập CategoryID thì lấy products theo category đầu tiên Của mảng Category(KindID == kindId)
                    products = productRepo.Products.OrderBy(p => p.ProductID).Where(p => p.CategoryID == categories.First().CategoryID);
                    ViewBag.SelectedCategory = categories.First().CategoryID;
                } else {
                    products = productRepo.Products.OrderBy(p => p.ProductName).Where(p => p.CategoryID == categoryID);
                    ViewBag.SelectedCategory = categoryID;
                }
            }
            kinds = kindRepo.Kinds;


            ViewData["CATEGORIES"] = categories;
            ViewData["PRODUCTS"] = products;
            ViewData["KINDS"] = kinds;
            return View();
        }


        public int NumberOfProduct(int categoryId) {
            int sum = productRepo.Products.Where(p => p.CategoryID == categoryId).Count();
            return sum;
        }
        public ViewResult EditProduct(int productId, string returnUrl) {
            Product product = productRepo.Products.FirstOrDefault(p => p.ProductID == productId);
            ViewData["CATEGORIES"] = categoryRepo.Categories;
            ViewData["PRODUCTSTATUSES"] = productStatusRepo.ProductStatus;
            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product product, HttpPostedFileBase imageProduct, string returnUrl) {
            if (ModelState.IsValid) {
                if (imageProduct != null) {
                    product.ImageMimeType = imageProduct.ContentType;
                    product.ImageData = new byte[imageProduct.ContentLength];
                    imageProduct.InputStream.Read(product.ImageData, 0, imageProduct.ContentLength);
                }
                productRepo.SaveProduct(product);
                if (returnUrl != null)
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("ManageProduct");
            }

            return RedirectToAction("EditProduct", new { productId = product.ProductID });
        }

    }
}