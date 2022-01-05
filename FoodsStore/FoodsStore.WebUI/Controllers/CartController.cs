using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using FoodsStore.WebUI.Mail;
using FoodsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;   
using System.Windows;

namespace FoodsStore.WebUI.Controllers
{

    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private IBillRepository billRepo;
        private IProductsInBillRepository productsInBillRepo;
        private IProductRepository productRepo;
        private IKindRepository kindRepo;
        private ICategoryRepository categoryRepo;

        public CartController(IBillRepository billRepo, IProductsInBillRepository productsInBillRepo, IProductRepository productRepo, IKindRepository kindRepo, ICategoryRepository categoryRepo)
        {
            this.billRepo = billRepo;
            this.productsInBillRepo = productsInBillRepo;
            this.productRepo = productRepo;
            this.kindRepo = kindRepo;
            this.categoryRepo = categoryRepo;
        }

        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddToCart(int productID, int quantity = 1)
        {
            CartDetailsModel cartDetailsModel = null;
            Bill userBill;
            try
            {
                Product product = GetProduct(productID);
                string username = HttpContext.User.Identity.Name;
                userBill = billRepo.GetUsersLastBillIsNotPay(username);
                if (userBill == null)
                {
                    userBill = CreateBillUser(username);
                }
                int totalQuantity = quantity;
                if (productsInBillRepo.CheckBillContainProduct(userBill.BillID, productID))
                {
                    totalQuantity = totalQuantity + productsInBillRepo.GetQuantityAProductInBill(userBill.BillID, productID);
                }
                int statusProdInStock = CheckProductIsAvailableWithQuantity(product, totalQuantity);
                if (statusProdInStock == 1)
                {
                    ProductsInBill productsInBill = new ProductsInBill();
                    productsInBill.BillID = userBill.BillID;
                    productsInBill.ProductID = productID;
                    productsInBill.ProductName = product.ProductName;
                    productsInBill.Quantity = quantity;
                    productsInBill.Price = product.Price;
                    productsInBill.Total = product.Price * quantity;
                    bool result = productsInBillRepo.InsertProductToBill(productsInBill);
                    if (result)
                    {
                        UpdateBillDetails(userBill.BillID);
                        TempData["MessageAddToCart"] = "Add to cart success!!";                        
                    }
                    else
                    {
                        TempData["MessageAddToCart"] = "Error to Add to cart!!";
                    }
                }
                else
                {
                    TempData["ErrorAddToCart"] = "Error to Add to cart!!";
                }
                cartDetailsModel = LoadCartDetailsModel(userBill.BillID);
            }catch(Exception ex)
            {
                SendMailSSL.SendErrorToAdmin("Error at AddToCart: " + ex.ToString());
            }                        
            return Json(cartDetailsModel, JsonRequestBehavior.AllowGet);
        }

        private void UpdateBillDetails(int BillID)
        {
            Bill bill = billRepo.GetBillDetails(BillID);
            bill.SubTotal = productsInBillRepo.GetToTalOfABill(BillID);
            bill.Tax = bill.SubTotal / 10;
            bill.Total = bill.Tax + bill.SubTotal;
            bill.LastTimeChange = DateTime.Now;
            billRepo.UpdateBill(bill);
        }

        public int GetQuantityOfProduct(int productID)
        {
            int result = 0;
            Product p = GetProduct(productID);
            if (p.StatusCode.Equals("STOC"))
            {
                result = p.Quantity;
            }else if (p.StatusCode.Equals("STOP"))
            {
                result = -1;
            }
            return result;
        }

        public int GetMaxQuantityCanAddToCart(int productID)
        {
            int result = 0;
            CartDetailsModel cart = (CartDetailsModel)Session["CartDetails"];
            if(cart != null)
            {               
                if (productsInBillRepo.CheckBillContainProduct(cart.BillID, productID))
                {
                    int quantityInCart = productsInBillRepo.GetQuantityAProductInBill(cart.BillID, productID);
                    result = productRepo.GetQuantity(productID) - quantityInCart;
                }
                else
                {
                    result = productRepo.GetQuantity(productID);
                }
            }
            else
            {
                result = productRepo.GetQuantity(productID);
            }
            return result;
        }

        [HttpPost]
        public string RemoveAProductInCart(int IDBill, int productID)
        {
            string prodName = null;
            try
            {
                bool result = productsInBillRepo.RemoveAProductInBill(IDBill, productID);
                if (result)
                {
                    prodName = productRepo.GetProductName(productID);
                    UpdateBill(IDBill);
                    LoadCartDetailsModel(IDBill);
                }
            }
            catch (Exception ex)
            {
                SendMailSSL.SendErrorToAdmin("Error at RemoveAProductInCart: " + ex.ToString());
            }
            return prodName;
        }

        public ActionResult LoadCartDetails(int IDBill = 0, string returnUrl = null)
        {
           
            CartDetailsModel cart = (CartDetailsModel)Session["CartDetails"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["returnUrl"] = returnUrl;
            }
            if(IDBill == 0 && cart !=null)
            {
                IDBill = cart.BillID;
            }
            try
            {   
                if(IDBill == 0)
                {
                    ViewBag.CartNull = "You have no product in cart !!";
                }
                else
                {
                    //if session timeout
                    if (cart == null)
                    {
                        LoadCartDetailsModel(IDBill);
                    }
                    if (cart != null)
                    { 
                        if (cart.ProductsInCart != null)
                        {
                            CheckCartIsChangeInStock(cart);
                            cart = LoadCartDetailsModel(IDBill);
                        }
                        else
                        {
                            ViewBag.CartNull = "You have no product in cart !!";
                        }                       
                    }
                    else
                    {
                        ViewBag.CartNull = "You have no product in cart !!";
                    }
                }                                            
            }
            catch (Exception ex)
            {
                SendMailSSL.SendErrorToAdmin("Error at LoadCartDetails: " + ex.ToString());
            }
            return View("ShoppingCart", cart);
        }

        private bool CheckCartIsChangeInStock(CartDetailsModel cart)
        {
            bool result = false;
            
            List<ProductInCartDetailsModel> productsInCart = cart.ProductsInCart;
            foreach (ProductInCartDetailsModel p in productsInCart)
            {
                ProductsInBill prodInBill = new ProductsInBill();
                prodInBill.BillID = cart.BillID;
                prodInBill.ProductID = p.ProductID;
                prodInBill.ProductName = productRepo.GetProductName(p.ProductID);
                prodInBill.Quantity = p.Quantity;
                if (p.Price != productRepo.GetUnitPrice(p.ProductID))
                {
                   // double oldPrice = p.Price;                    
                    prodInBill.Price = productRepo.GetUnitPrice(p.ProductID);
                    p.Price = prodInBill.Price;
                    prodInBill.Total = p.Quantity * prodInBill.Price;
                    p.Total = prodInBill.Total;                    
                }
                else
                {
                    prodInBill.Price = p.Price;
                    prodInBill.Total = p.Total;
                }
                result = productsInBillRepo.UpdateProductInBill(prodInBill);
            }
            UpdateBillDetails(cart.BillID);

            return result;
        }

        public int UpdateCart(int IDBill, int productID, int quantity)
        {
            Product p = productRepo.GetProduct(productID);
            int result = 0;
            try
            {
                result = CheckProductIsAvailableWithQuantity(p, quantity);
                if(result == 1)
                {
                    ProductsInBill prodInBill = productsInBillRepo.GetDetailsAProductInBill(IDBill, productID);
                    prodInBill.Quantity = quantity;
                    prodInBill.Total = quantity * prodInBill.Price;
                    productsInBillRepo.UpdateProductInBill(prodInBill);
                    UpdateBillDetails(IDBill);
                    LoadCartDetailsModel(IDBill);
                }
            }
            catch (Exception ex)
            {
                SendMailSSL.SendErrorToAdmin("Error at UpdateCart: " + ex.ToString());
                result = -1;
            }
            return result;
        }

        private Product GetProduct(int productId) => productRepo.Products.FirstOrDefault(p => p.ProductID == productId);

        private Bill CreateBillUser(string username)
        {
            Bill result = billRepo.CreateBillForUser(username);
            billRepo.InsertBill(result);
            return result;
        }

        private int CheckProductIsAvailableWithQuantity(Product p, int quantity)
        {
            int result = 0;
            if (p.StatusCode.Equals("OUTS") || p.Quantity==0)
            {
                TempData["ErrorQuantity"] = "This product is out of stock !";
                result = 0;
            }
            else if (p.Quantity < quantity)
            {
                TempData["ErrorQuantity"] = "This product is out of stock !"+" Only "+p.Quantity+" left on stock!!";
                result = 0;
            }
            else if (p.StatusCode.Equals("STOP"))
            {
                TempData["ErrorQuantity"] = "This product is stop selling !!";
                result = 2;
            }
            else
            {
                result = 1;
            }
            return result;
        }

        private bool CheckAvailableAProductInCart(int productID, int currQuantityProdInBill)
        {
            bool result = false;
            Product prod = productRepo.Products.FirstOrDefault(p => p.ProductID == productID);
            if (prod.StatusCode.Equals("OUTS"))
            {
                TempData["ErrorQuantity"] = "This product is out of stock !";
            }
            else if (prod.StatusCode.Equals("STOP"))
            {
                TempData["ErrorQuantity"] = "This product is stop selling !!";
            }
            return result;
        }

        private int GetQuantityAProdInBill(int billID,int proID)
        {
            return productsInBillRepo.GetQuantityAProductInBill(billID, proID);
        }

        public JsonResult CheckCartIsValidToCheckOut(int IDBill)
        {
            CartDetailsStatusModel cartDetailsStatus = new CartDetailsStatusModel();
            IEnumerable<ProductsInBill> listProductsInBills = productsInBillRepo.GetListProductInBill(IDBill);
            try
            {
                foreach (ProductsInBill p in listProductsInBills)
                {
                    //check status
                    string statusProd = productRepo.GetStatus(p.ProductID);
                    if (statusProd.Equals("STOP"))
                    {                       
                        cartDetailsStatus.ProductID = p.ProductID;
                        cartDetailsStatus.ProductName = p.ProductName;
                        cartDetailsStatus.Problem = "STOP";
                        return Json(cartDetailsStatus,JsonRequestBehavior.AllowGet);
                    }
                    if (statusProd.Equals("OUTS"))
                    {
                        cartDetailsStatus.ProductID = p.ProductID;
                        cartDetailsStatus.ProductName = p.ProductName;
                        cartDetailsStatus.Problem = "OUTS";
                        return Json(cartDetailsStatus, JsonRequestBehavior.AllowGet);
                    }
                    int quantityInCart = p.Quantity;
                    int quantityInStock = productRepo.GetQuantity(p.ProductID);            
                    if (quantityInStock < quantityInCart)
                    {                       
                        cartDetailsStatus.ProductID = p.ProductID;
                        cartDetailsStatus.ProductName = p.ProductName;
                        cartDetailsStatus.Problem = "HIGHER";
                        cartDetailsStatus.QuantityInStock = quantityInStock;
                        return Json(cartDetailsStatus, JsonRequestBehavior.AllowGet);
                    }
                    double priceProdInCart = p.Price;
                    double priceProdInStock = productRepo.GetUnitPrice(p.ProductID);
                    if (priceProdInCart != priceProdInStock)
                    {                        
                        cartDetailsStatus.ProductID = p.ProductID;
                        cartDetailsStatus.ProductName = p.ProductName;
                        cartDetailsStatus.Problem = "CHANGEPRICE";
                        cartDetailsStatus.NewPrice = priceProdInStock;
                        cartDetailsStatus.OldPrice = priceProdInCart;
                        return Json(cartDetailsStatus, JsonRequestBehavior.AllowGet);
                    }
                }
            }catch (Exception ex)
            {
                SendMailSSL.SendErrorToAdmin("Error at CheckCartIsValidToCheckOut: " + ex.ToString());
            }
            return Json(cartDetailsStatus, JsonRequestBehavior.AllowGet);
        }

        //load at time user login, check user has bill that is not payed.
        public void LoadCartDetailsUser()
        {
            string username = HttpContext.User.Identity.Name;
            Bill userBill = billRepo.GetUsersLastBillIsNotPay(username);
            if (userBill != null)
            {
                LoadCartDetailsModel(userBill.BillID);
            }
        }
        //load cart for shopping cart hover
        private CartDetailsModel LoadCartDetailsModel(int IDBill)
        {
            CartDetailsModel cart = new CartDetailsModel();
            try
            {
                Bill bill = billRepo.GetBillDetails(IDBill);
                IEnumerable<ProductsInBill> productsInBills = productsInBillRepo.GetListProductInBill(IDBill);
                cart.BillID = IDBill;
                cart.SubTotal = bill.SubTotal;
                cart.Tax = bill.Tax;
                cart.Total = bill.Total;
                cart.LastTimeChange = bill.LastTimeChange.ToString("dd/MM/yyyy HH:mm:ss");
                int numProd = 0;
                if (productsInBills != null && productsInBills.Any())
                    foreach (ProductsInBill p in productsInBills)
                    {
                        ProductInCartDetailsModel pInCart = new ProductInCartDetailsModel();
                        pInCart.ProductID = p.ProductID;
                        pInCart.ProductName = p.ProductName;
                        pInCart.Category = GetProductCategoryName(p.ProductID);
                        pInCart.Kind = GetProductKindName(p.ProductID);
                        pInCart.Quantity = p.Quantity;
                        pInCart.Price = p.Price;
                        pInCart.Total = p.Total;
                        pInCart.ShortDescription = productRepo.GetShortDescription(p.ProductID);
                        pInCart.StatusCode = productRepo.GetStatus(p.ProductID);
                        if (cart.ProductsInCart == null)
                        {
                            cart.ProductsInCart = new List<ProductInCartDetailsModel>();
                        }
                        cart.ProductsInCart.Add(pInCart);
                        numProd = numProd + p.Quantity;
                    }
                cart.QuantityProduct = numProd;
                Session["CartDetails"] = cart;
            }catch(Exception ex)          
            {
                SendMailSSL.SendErrorToAdmin("Error at LoadCartDetailsModel(CartControler): " + ex.ToString());
            }
            
            return cart;
        }
        
        private void UpdateBill(int IDBill)
        {
            Bill bNew = billRepo.GetBillDetails(IDBill);
            if (bNew != null)
            {
                bNew.SubTotal = productsInBillRepo.GetToTalOfABill(IDBill);
                bNew.Tax = bNew.SubTotal / 10;
                bNew.Total = bNew.SubTotal + bNew.Tax;
                bNew.LastTimeChange = DateTime.Now;
                billRepo.UpdateBill(bNew);
            }
        }       

        public string GetProductCategoryName(int productId)
        {
            Product product = GetProduct(productId);
            if (product != null)
            {
                int categoryId = product.CategoryID;
                return categoryRepo.Categories.FirstOrDefault(c => c.CategoryID == categoryId).CategoryName;
            }
            return null;
        }

        public string GetProductKindName(int productId)
        {
            Product p = GetProduct(productId);
            int kindId = categoryRepo.Categories.FirstOrDefault(c => c.CategoryID == p.CategoryID).KindID;
            return kindRepo.Kinds.FirstOrDefault(k => k.KindID == kindId).KindOfProduct;
        }
    }
}