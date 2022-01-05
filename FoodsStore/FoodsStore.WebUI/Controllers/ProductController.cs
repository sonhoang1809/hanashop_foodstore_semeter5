using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using FoodsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace FoodsStore.WebUI.Controllers {
    public class ProductController : Controller {

        private IProductRepository productRepo;
        private IKindRepository kindRepo;
        private ICategoryRepository categoryRepo;
        private IProductStatusRepository productStatusRepo;

        private int PAGE_SIZE = int.Parse(ConfigurationManager.AppSettings["PageSize"]);

        public ProductController(IProductRepository productRepo, IKindRepository kindRepo, ICategoryRepository categoryRepo, IProductStatusRepository productStatusRepo) {
            this.productRepo = productRepo;
            this.kindRepo = kindRepo;
            this.categoryRepo = categoryRepo;
            this.productStatusRepo = productStatusRepo;
        }

        public ActionResult Index(int page = 1) {
            var Products = productRepo.Products.OrderByDescending(p => p.ProductName)
                                                            .Where(p => p.StatusCode.Equals("STOC"));
            ProductListViewModel model = new ProductListViewModel {
                Products = Products.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE),
                PagingInfo = new PagingInfo {
                    TotalItems = Products.Count(),
                    CurrentPage = page,
                    ItemPerPage = PAGE_SIZE
                },
                Kinds = kindRepo.Kinds,
                Categories = null,
                SearchInfo = null

            };

            return View(model: model);
        }

        public ActionResult ContinueShopping(string returnUrl = null) {

            if (string.IsNullOrEmpty(returnUrl)) {
                return Index();
            } else {
                return Redirect(returnUrl);
            }
        }

        public ActionResult SearchProduct(int? kindId, int? categoryId, float? minPrice, float? maxPrice, string productName, int page = 1) {
            IEnumerable<Product> list = productRepo.Products.Where(p => p.StatusCode.Equals("STOC"));
            IEnumerable<Category> categories = null;
            if (kindId != null) {
                categories = categoryRepo.Categories.Where(c => c.KindID == kindId);
                list = from prod in list
                       join cate in categories on prod.CategoryID equals cate.CategoryID
                       select prod;
            }
            if (categoryId != null) {
                list = list.Where(p => p.CategoryID == categoryId);
            }
            if (minPrice != null) {
                list = list.Where(p => p.Price >= minPrice);
            }
            if (maxPrice != null) {
                list = list.Where(p => p.Price <= maxPrice);
            }

            if (!string.IsNullOrEmpty(productName)) {
                list = list.Where(p => p.ProductName.ToLower().Contains(productName.Trim().ToLower())
                                            || p.Description.ToLower().Contains(productName.Trim().ToLower()));
            }

            ProductListViewModel model = new ProductListViewModel {
                Products = list.OrderByDescending(p => p.DateCreated)
                                  .Skip((page - 1) * PAGE_SIZE)
                                  .Take(PAGE_SIZE),
                PagingInfo = new PagingInfo {
                    TotalItems = list.Count(),
                    CurrentPage = page,
                    ItemPerPage = PAGE_SIZE
                },
                Kinds = kindRepo.Kinds,
                Categories = categories,
                SearchInfo = new SearchInfo {
                    KindID = kindId,
                    CategoryID = categoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    ProductName = productName
                }
            };

            if (model.Products.Count() == 0) {
                ViewBag.SearchNull = "Result not found !!";
            }
            return View("Index", model);
        }

        public JsonResult SearchProductJson(int? kindId, int? categoryId, float? minPrice, float? maxPrice, string productName, int page = 1) {
            try {
                IEnumerable<Product> list = productRepo.Products.Where(pro => pro.StatusCode.Equals("STOC"));
                IEnumerable<Category> categories = null;

                if (kindId != null) {
                    categories = categoryRepo.Categories.Where(c => c.KindID == kindId);
                    list = from prod in list
                           join cate in categories on prod.CategoryID equals cate.CategoryID
                           select prod;
                }

                if (categoryId != null) {
                    list = list.Where(p => p.CategoryID == categoryId);
                }

                if (minPrice != null) {
                    list = list.Where(p => p.Price >= minPrice);
                }

                if (maxPrice != null) {
                    list = list.Where(p => p.Price <= maxPrice);
                }

                if (!string.IsNullOrEmpty(productName)) {
                    list = list.Where(p => p.ProductName.ToLower().Contains(productName.Trim().ToLower())
                                                || p.Description.ToLower().Contains(productName.Trim().ToLower()));
                }

                ProductListViewModel model = new ProductListViewModel {
                    Products = list.OrderByDescending(p => p.DateCreated)
                                   .Skip((page - 1) * PAGE_SIZE)
                                   .Take(PAGE_SIZE),
                    PagingInfo = new PagingInfo {
                        TotalItems = list.Count(),
                        CurrentPage = page,
                        ItemPerPage = PAGE_SIZE
                    },
                    Kinds = kindRepo.Kinds,
                    Categories = categories,
                    SearchInfo = new SearchInfo {
                        KindID = kindId,
                        CategoryID = categoryId,
                        MinPrice = minPrice,
                        MaxPrice = maxPrice,
                        ProductName = productName
                    }
                };
                model.NumProduct = model.Products.Count();
                if (model.Products.Count() == 0) {
                    ViewBag.SearchNull = "Result not found !!";
                }
                var result = Json(model, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            } catch (Exception e) {
                MessageBox.Show(e.ToString());
            }

            return null;
        }

        private Product GetProduct(int productId) => productRepo.Products.FirstOrDefault(p => p.ProductID == productId);

        public FileContentResult GetImageProduct(int productId) {
            return File(productRepo.GetImgData(productId), productRepo.GetMimeType(productId));
        }

        public JsonResult GetImageProductJson(int productId) {
            Product prod = productRepo.Products.FirstOrDefault(p => p.ProductID == productId);
            if (prod != null) {
                if (prod.ImageData != null && !string.IsNullOrEmpty(prod.ImageMimeType)) {
                    return Json(prod.ImageData, prod.ImageMimeType);
                }
            }
            return null;
        }

        public string GetProductCategoryName(int productId) {
            Product product = GetProduct(productId);
            if (product != null) {
                int categoryId = product.CategoryID;
                return categoryRepo.Categories.FirstOrDefault(c => c.CategoryID == categoryId).CategoryName;
            }
            return null;
        }

        public string GetProductKindName(int productId) {
            Product p = GetProduct(productId);
            int kindId = categoryRepo.Categories.FirstOrDefault(c => c.CategoryID == p.CategoryID).KindID;
            return kindRepo.Kinds.FirstOrDefault(k => k.KindID == kindId).KindOfProduct;
        }

        public string GetProductStatusName(int productId) {
            Product p = GetProduct(productId);
            return productStatusRepo.ProductStatus.FirstOrDefault(s => s.StatusCode == p.StatusCode).Status;
        }

        public ViewResult GetLatestProducts(int quantity) {
            IEnumerable<Product> list = productRepo.Products.OrderByDescending(p => p.DateCreated).Where(p => p.StatusCode.Equals("STOC")).Take(quantity);
            return View("_latestProductsPartialView", list);
        }


        public ViewResult GetSameCategoryOfProducts(int productId, int quantityProduct) {
            int categoryId = productRepo.Products.FirstOrDefault(p => p.ProductID == productId).CategoryID;
            IEnumerable<Product> list = productRepo.Products
                                                .Where(p => p.CategoryID == categoryId && p.StatusCode.Equals("STOC"))
                                                .SkipWhile(p => p.ProductID == productId)
                                                .Take(quantityProduct);
            return View("_recommendProductPartialView", list);
        }
        public ActionResult Details(int productId, string returnUrl) {
            if (User.Identity.IsAuthenticated) { //access denied with admin.
                if (User.IsInRole("Admin")) {
                    return RedirectToAction("EditProduct", "Admin", new { productId = productId, returnUrl = "/Product/Index" });
                }
            }
            if (productRepo.CheckProductIsAvailableInStock(productId))
            {
                return View(GetProduct(productId));
            }
            else
            {
                TempData["InavailableProd"] = "Oops, Sorry!! This product is out of stock!";
                return Redirect(returnUrl);
            }
           
        }
    }
}