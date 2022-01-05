using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using FoodsStore.WebUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodsStore.WebUI.Controllers {

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private IProductRepository productRepo;
        private IKindRepository kindRepo;
        private IProductStatusRepository productStatusRepo;
        private ICategoryRepository categoryRepo;
        private IUserRepository userRepo;
        private IRoleRepository roleRepo;
        private IBillRepository billRepo;
        private IProductsInBillRepository productsInBillRepo;

        private const int USER_PAGE_SIZE = 20; //Paging Manage User Page

        public AdminController(IProductRepository productRepo, IKindRepository kindRepo, IProductStatusRepository productStatusRepo, ICategoryRepository categoryRepo, IUserRepository userRepo, IRoleRepository roleRepo, IBillRepository billRepo, IProductsInBillRepository productsInBillRepo)
        {
            this.productRepo = productRepo;
            this.kindRepo = kindRepo;
            this.productStatusRepo = productStatusRepo;
            this.categoryRepo = categoryRepo;
            this.userRepo = userRepo;
            this.roleRepo = roleRepo;
            this.billRepo = billRepo;
            this.productsInBillRepo = productsInBillRepo;
        }


        public ActionResult ManageProduct(int? kindID, int? categoryID) {

            int? tempKindID = null;
            ProductListEditModel model = new ProductListEditModel();

            if (kindID == null && categoryID == null) {//Get products follow first category of first kind

                tempKindID = kindRepo.Kinds.First().KindID;
                model.Categories = categoryRepo.Categories.Where(c => c.KindID == tempKindID);
                model.Products = productRepo.Products.OrderBy(p => p.ProductName)
                                                     .Where(p => p.CategoryID == model.Categories.First().CategoryID);
                model.SelectedKind = tempKindID;
                model.SelectedCategory = model.Categories.First().CategoryID;

            } else {

                if (kindID == null && categoryID != null) {//Get products follow category
                    tempKindID = categoryRepo.Categories.FirstOrDefault(c => c.CategoryID == categoryID).KindID;
                    model.Categories = categoryRepo.Categories.Where(c => c.KindID == tempKindID);
                    model.SelectedKind = tempKindID;
                } else {
                    model.Categories = categoryRepo.Categories.Where(c => c.KindID == kindID);
                    model.SelectedKind = kindID;
                }
                if (kindID != null && categoryID == null) {//Get products of first category of kind
                    model.Products = productRepo.Products.OrderBy(p => p.ProductID).Where(p => p.CategoryID == model.Categories.First().CategoryID);
                    model.SelectedCategory = model.Categories.First().CategoryID;
                } else {
                    model.Products = productRepo.Products.OrderBy(p => p.ProductName).Where(p => p.CategoryID == categoryID);
                    model.SelectedCategory = categoryID;
                }
            }
            model.Kinds = kindRepo.Kinds;
            return View(model);
        }

        public int NumberOfProduct(int categoryId) {
            int sum = productRepo.Products.Where(p => p.CategoryID == categoryId).Count();
            return sum;
        }

        public ViewResult EditProduct(int productId, string returnUrl) {
            Product product = productRepo.Products.FirstOrDefault(p => p.ProductID == productId);
            ViewBag.returnUrl = returnUrl;
            ViewData["CATEGORIES"] = categoryRepo.Categories;
            ViewData["PRODUCTSTATUSES"] = productStatusRepo.ProductStatus;
            return View(product);
        }

        [HttpPost]
        public ActionResult SaveProduct(Product product, HttpPostedFileBase imageProduct, string returnUrl) {
            Product temp = productRepo.Products.FirstOrDefault(p => p.ProductID == product.ProductID);
            if(temp != null) {
                product.DateCreated = temp.DateCreated;
                product.DateModified = temp.DateModified;
                product.UserModified = temp.UserModified;
            }
            if (ModelState.IsValid) {
                if (imageProduct != null) {
                    product.ImageMimeType = imageProduct.ContentType;
                    product.ImageData = new byte[imageProduct.ContentLength];
                    imageProduct.InputStream.Read(product.ImageData, 0, imageProduct.ContentLength);
                }
                if (product.ProductID != 0) { //For Update Product
                    product.DateModified = DateTime.Now;
                    product.UserModified = userRepo.Users.FirstOrDefault(u => u.Username.Equals(HttpContext.User.Identity.Name)).FullName;
                    product.DateCreated = productRepo.Products.FirstOrDefault(p => p.ProductID == product.ProductID).DateCreated; //For return the view
                } else { //For Create Product
                    product.DateCreated = DateTime.Now;
                }
                ViewBag.StatusUpdate = "Successful!";
                productRepo.SaveProduct(product);
            }

            ViewBag.returnUrl = returnUrl;
            ViewData["CATEGORIES"] = categoryRepo.Categories;
            ViewData["PRODUCTSTATUSES"] = productStatusRepo.ProductStatus;
            return View("EditProduct", product);
        }

        public ViewResult CreateProduct(string returnUrl) {
            Product product = new Product {
                DateCreated = DateTime.Now
            };
            ViewBag.returnUrl = returnUrl;
            ViewData["CATEGORIES"] = categoryRepo.Categories;
            ViewData["PRODUCTSTATUSES"] = productStatusRepo.ProductStatus;
            return View("EditProduct", product);
        }


        /*----------------------------USER MANAGER------------------------------*/

        public ActionResult ManageUser(string returnUrl, string roleId = "US", int page = 1) {
            UserListEditModel model = new UserListEditModel() {
                Roles = roleRepo.Roles,
                Users = userRepo.Users.Where(u => u.RoleID.Equals(roleId))
                                      .OrderBy(u => u.Username)
                                      .Skip((page - 1) * USER_PAGE_SIZE)
                                      .Take(USER_PAGE_SIZE),
                SelectedRoleId = roleId,
                PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemPerPage = USER_PAGE_SIZE,
                    TotalItems = userRepo.Users.Where(u => u.RoleID.Equals(roleId)).Count()
                }
            };
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        public ViewResult EditProfileUser(string username, string returnUrl) {
            ViewBag.isCreateUser = false;
            ViewBag.returnUrl = returnUrl;
            ViewData["ROLES"] = roleRepo.Roles;
            User user = userRepo.Users.FirstOrDefault(u => u.Username.Equals(username));
            return View(user);
        }

        public ViewResult CreateUser(string returnUrl) {
            ViewBag.isCreateUser = true;
            ViewBag.returnUrl = returnUrl;
            ViewData["ROLES"] = roleRepo.Roles;
            return View("EditProfileUser",null);
        }

        public ViewResult SaveProfileUser(User user, bool isCreateUser, HttpPostedFileBase avatar, string returnUrl) {
            //Check email is existed
            bool isDuplicatedEmail = false;
            if (!string.IsNullOrEmpty(user.Email)) {
                isDuplicatedEmail = userRepo.Users
                    .Where(u => u.Email != null)
                    .FirstOrDefault(u => u.Email.Equals(user.Email) && !u.Username.Equals(user.Username)) != null;
            }
            if (isDuplicatedEmail) {
                ModelState.AddModelError("duplicatedEmail", "Email is existed!");
            }
            //Check avatar != null
            if (avatar != null) {
                user.ImageData = new byte[avatar.ContentLength];
                avatar.InputStream.Read(user.ImageData, 0, avatar.ContentLength);
                user.ImageMimeType = avatar.ContentType;
            }
            if (isCreateUser) { //Create User
                user.DateCreated = DateTime.Now;
                
                if (ModelState.IsValid) {
                    bool isDuplicatedUsername = userRepo.Users.FirstOrDefault(u => u.Username.Equals(user.Username)) != null;

                    if (isDuplicatedUsername) {
                        ModelState.AddModelError("duplicatedUsername", "Username is existed!");
                    }
                    if(!isDuplicatedUsername && !isDuplicatedEmail){
                        userRepo.InsertUser(user);
                        ViewBag.StatusUpdate = "Successful!";
                        isCreateUser = false;
                    }
                }
            } else { //Update User
                if (ModelState.IsValidField("FullName") && ModelState.IsValidField("Description") && 
                    ModelState.IsValidField("RoleID") && ModelState.IsValidField("Email") && !isDuplicatedEmail) {
                    userRepo.UpdateUser(user);
                    ViewBag.StatusUpdate = "Successful!";
                }
            }
            ViewBag.isCreateUser = isCreateUser;
            ViewBag.returnUrl = returnUrl;
            ViewData["ROLES"] = roleRepo.Roles;
            return View("EditProfileUser", user);
        }

        /*----------------------------DASHBOARD------------------------------*/

        public ActionResult Dashboard()
        {
            GraphDashBoardModel result = DrawDashboard();
            return View("Dashboard", result);
        }

        private GraphDashBoardModel DrawDashboard(string dateStr = null)
        {
            GraphDashBoardModel result = new GraphDashBoardModel();
            DateTime date;
            if (dateStr == null)
            {
                date = DateTime.Now.Date.AddDays(-6);
            }
            else
            {
                date = DateTime.Parse(dateStr);
            }
            result.ListNumberBillEachDay = DrawNumberOrderBill(date);//draw bill
            result.ListRevenueEachDay = DrawListRevenue(date);//draw revenue
            result.ListDate = GetListDate(date);// date   
            result.ListQuantityProductInStorageHasCategory = DrawCategoryInStorage();
            result.ListQuantityProductInStorage = DrawKindInStorage();
            return result;
        }

        private List<string> GetListDate(DateTime date)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < 7; i++)
            {
                result.Add(date.ToString("dd-MM"));
                date = date.AddDays(1);
            }
            return result;
        }

        //Draw number of order bill each 7 days
        //autoget
        public JsonResult DrawDashboardJson(string dateStr)
        {
            GraphDashBoardModel result = new GraphDashBoardModel();
            DateTime date = DateTime.Parse(dateStr);
            result.ListNumberBillEachDay = DrawNumberOrderBill(date);
            result.ListRevenueEachDay = DrawListRevenue(date);
            result.ListDate = GetListDate(date);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Draw number of order bill each 7 days
        //autoget
        public JsonResult DrawDashboardAutoJson()
        {
            GraphDashBoardModel result = new GraphDashBoardModel();
            DateTime date= DateTime.Now.Date;
            result.NumberBillToday = billRepo.GetNumberOfBillIsOrderedOnDay(date);
            result.RevenueToday = billRepo.GetRevenueOnDay(date);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<int> DrawNumberOrderBill(DateTime date)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                result.Add(billRepo.GetNumberOfBillIsOrderedOnDay(date));
                date = date.AddDays(1);
            }
            return result;
        }

        private List<double> DrawListRevenue(DateTime date)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < 7; i++)
            {
                result.Add(billRepo.GetRevenueOnDay(date));
                date = date.AddDays(1);
            }
            return result;
        }

        private Hashtable DrawCategory()
        {
            Hashtable result = new Hashtable();
            IEnumerable<Category> listCate = categoryRepo.Categories;
            IEnumerable<int> listOrderedBillID = billRepo.GetListBillIdIsPayed();
            if(listCate!=null && listOrderedBillID != null)
            {
                foreach (Category c in listCate)
                {
                    IEnumerable<int> listIDProductHasCate = productRepo.GetListIDProductHasCategory(c.CategoryID);
                    int numProductIsSoldHasCate = productsInBillRepo.GetNumProductIsSoldHasCate(listOrderedBillID, listIDProductHasCate);
                    result.Add(c.CategoryName, numProductIsSoldHasCate);
                }
            }           
            return result;
        }

        private Hashtable DrawCategoryInStorage()
        {
            Hashtable result = new Hashtable();
            IEnumerable<Category> listCate = categoryRepo.Categories;
            if (listCate != null)
            {
                foreach (Category c in listCate)
                {
                    int numberProductBelongToCategory = productRepo.CountNumberProductHasCategory(c.CategoryID);
                    result.Add(c.CategoryName, numberProductBelongToCategory);
                }
            }
            return result;
        }

        //Kind food in storage
        private Hashtable DrawKindInStorage()
        {
            Hashtable result = new Hashtable();
            IEnumerable<Kind> listKind = kindRepo.Kinds;
            if (listKind != null)
            {
                foreach (Kind k in listKind)
                {
                    IEnumerable<int> ListCategoryBelongToKind = categoryRepo.GetListCategoryBelongToKind(k.KindID);
                    int numberProductBelongToKind = productRepo.CountNumberProductHasCategory(ListCategoryBelongToKind);
                    result.Add(k.KindOfProduct, numberProductBelongToKind);
                }
            }       
            return result;
        }
    }
}