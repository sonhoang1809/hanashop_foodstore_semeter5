using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using FoodsStore.WebUI.Mail;
using FoodsStore.WebUI.Models;
using FoodsStore.WebUI.Paypal;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Windows;

namespace FoodsStore.WebUI.Controllers
{
    [Authorize(Roles = "User")]
    public class CheckOutController : Controller
    {
        private IBillRepository billRepo;
        private IProductsInBillRepository productsInBillRepo;
        private IProductRepository productRepo;
        private IKindRepository kindRepo;
        private ICategoryRepository categoryRepo;
        private IUserRepository userRepo;

        public CheckOutController(IBillRepository billRepo, IProductsInBillRepository productsInBillRepo, IProductRepository productRepo, IKindRepository kindRepo, ICategoryRepository categoryRepo, IUserRepository userRepo)
        {
            this.billRepo = billRepo;
            this.productsInBillRepo = productsInBillRepo;
            this.productRepo = productRepo;
            this.kindRepo = kindRepo;
            this.categoryRepo = categoryRepo;
            this.userRepo = userRepo;
        }

        // GET: CheckOut
        public ActionResult Index(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View("CheckOut");
        }

        public ActionResult ViewOrderDetails(int IDBill)
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
            }
            catch (Exception e)
            {
                SendMailSSL.SendErrorToAdmin("Error at ViewOrderDetails: "+ e.ToString());
            }
            return View("OrderDetails", cart);
        }
        
        public ActionResult PaymentMethod(string fullname, string address,string street,string city,string phone)
        {
            return View("Method-payment");
        }
        
        private string GetProductCategoryName(int productId)
        {
            Product product = productRepo.GetProduct(productId);
            if (product != null)
            {
                int categoryId = product.CategoryID;
                return categoryRepo.Categories.FirstOrDefault(c => c.CategoryID == categoryId).CategoryName;
            }
            return null;
        }

        private string GetProductKindName(int productId)
        {
            Product p = productRepo.GetProduct(productId);
            int kindId = categoryRepo.Categories.FirstOrDefault(c => c.CategoryID == p.CategoryID).KindID;
            return kindRepo.Kinds.FirstOrDefault(k => k.KindID == kindId).KindOfProduct;
        }
      
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
            }
            catch (Exception e)
            {
                SendMailSSL.SendErrorToAdmin("Error at LoadCartDetailsModel: " + e.ToString());
            }
            return cart;
        }

        
        public async Task<JsonResult> CreateOrderPaypal(string fullname = "Son Hoang dep trai", string address="123/456", string street="18 street", string city = "HCM", int IDBill = 69)
        {          
            try
            {
                //string strHost = HttpContext.Current.Request.Url.Host.ToString();
                CartDetailsModel cart = LoadCartDetailsModel(IDBill);              
                var request = new OrdersCreateRequest();
                request.Prefer("return=representation");
                request.RequestBody(CreateOrderSample.BuildRequestBody(fullname, address, street, city, cart));
                var response = await PayPalClient.client().Execute(request);               
                var createOrderResult = response.Result<Order>();                
                string orderId = createOrderResult.Id;
                //string url = "https://www.sandbox.paypal.com/checkoutnow?token=" + orderId;
                //Response.Redirect(url);
                var result = Json(createOrderResult, "application/json", JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
              
            }
            catch (Exception e)
            {
                SendMailSSL.SendErrorToAdmin("Error at CreateOrderPaypal: " + e.ToString());
            }
            return null;
        }


        //public Task<PayPalHttp.HttpResponse> CreateOrderPaypal2()
        //{
        //     return CreateOrderSample.CreateOrder(true);
        // }
        private bool CheckProductInCart(CartDetailsModel cart)
        {
            bool result = true;
            foreach(ProductInCartDetailsModel pInCart in cart.ProductsInCart)
            {
                Product prod = productRepo.Products.FirstOrDefault(p => p.ProductID == pInCart.ProductID);
                if (prod.StatusCode.Equals("OUTS") || prod.StatusCode.Equals("STOP") || prod.Quantity < pInCart.Quantity|| prod.Price != pInCart.Price)
                {
                    result = false;
                    break;
                }
            }
            
            return result;
        }
        
        public async Task<ActionResult> AuthorizeCaptureOrderPaypal()
        {
            CartDetailsModel cart = (CartDetailsModel)Session["CartDetails"];
            try
            {
                if (CheckProductInCart(cart))
                {
                    //neu on het thi tien hanh thanh toan
                    string orderId = Request["token"];
                    var requestAuthorization = new OrdersAuthorizeRequest(orderId);
                    requestAuthorization.Prefer("return=representation");
                    requestAuthorization.RequestBody(new AuthorizeRequest());
                    var responseAuthorization = await PayPalClient.client().Execute(requestAuthorization);
                    var resultAuthorization = responseAuthorization.Result<Order>();
                    if (resultAuthorization.Status.Equals("COMPLETED"))
                    {
                        //if success
                        //get authorizationID
                        string AuthorizationId = resultAuthorization.PurchaseUnits[0].Payments.Authorizations[0].Id;
                        var requestCapture = new AuthorizationsCaptureRequest(AuthorizationId);
                        requestCapture.Prefer("return=representation");
                        requestCapture.RequestBody(new CaptureRequest());
                        //execute capture
                        var responseCapture = await PayPalClient.client().Execute(requestCapture);
                        var captureOrderResult = responseCapture.Result<PayPalCheckoutSdk.Payments.Capture>();
                        if (captureOrderResult.Status.Equals("PENDING"))//success
                        {
                            //check lai xem con hang trong kho k sau khi customer da thanh toan xong
                            //neu k thi refund tien r tra ve trang shopping-cart
                            if (!CheckProductInCart(cart))
                            {
                                await RefundOrderPaypal(captureOrderResult.Id);
                                return RedirectToAction("LoadCartDetails", "Cart");
                            }
                            else
                            {
                                return FinishOrderPaypal(cart);
                            }                            
                        }
                        else
                        {
                            TempData["ErrorOrder"] = "Error to order by PayPal!!";
                            return RedirectToAction("LoadCartDetails", "Cart");
                        }
                    }
                    else
                    {
                        TempData["ErrorOrder"] = "Error to order by PayPal!!";
                        return RedirectToAction("LoadCartDetails", "Cart");
                    }
                }
                else
                {
                    TempData["ErrorOrder"] = "Error to order by PayPal!!";
                    return RedirectToAction("LoadCartDetails", "Cart");
                }               
            }
            catch (Exception e)
            {
                SendMailSSL.SendErrorToAdmin("Error at AuthorizeCaptureOrderPaypal: " + e.ToString());
            }
            return RedirectToAction("LoadCartDetails", "Cart");
        }

        public async Task RefundOrderPaypal(string captureOrderId)
        {
            try
            {
                CartDetailsModel cart = (CartDetailsModel)Session["CartDetails"];
                double total = cart.Total;
                var request = new CapturesRefundRequest(captureOrderId);
                request.Prefer("return=representation");
                RefundRequest refundRequest = new RefundRequest()
                {
                    Amount = new PayPalCheckoutSdk.Payments.Money
                    {
                        Value = "" + total,
                        CurrencyCode = "USD"
                    }
                };
                request.RequestBody(refundRequest);
                var response = await PayPalClient.client().Execute(request);
                var result = response.Result<PayPalCheckoutSdk.Payments.Refund>();
                if (result.Status.Equals("COMPLETED"))
                {
                    TempData["RefundMess"] = "Refund money success !!";
                }
            }
            catch (Exception e)
            {
                SendMailSSL.SendErrorToAdmin("Error at RefundOrderPaypal: " + e.ToString());
            }            
        }

        public ActionResult FinishOrderPaypal(CartDetailsModel cart)
        {
            //update product
            foreach (ProductInCartDetailsModel pro in cart.ProductsInCart)
            {
                Product p = productRepo.GetProduct(pro.ProductID);
                int newQuantity = p.Quantity - pro.Quantity;
                p.Quantity = newQuantity;
                if (newQuantity == 0)
                {
                    p.StatusCode = "OUTS";
                }
                productRepo.UpdateOrderedProduct(p);
            }
            //update bill status ->payed
            billRepo.UpdateBillStatus(cart.BillID, "PAY");
            //send mail for user here
            Session.Remove("CartDetails");
            TempData["PaymentMessage"] = "Your bill(bill ID:" + cart.BillID + ") is ordered success!!";
            TempData["BillID"] = cart.BillID;

            string username = HttpContext.User.Identity.Name;
            User user = userRepo.GetUser(username);
            //send mail to user
            SendMailSSL.SendOrderedBillPayPalToUser(user.Email, cart, user.FullName);
            return RedirectToAction("Index", "Product");
        }

        public ActionResult FinishOrderCOD(int IDBill)
        {
            CartDetailsModel cart = LoadCartDetailsModel(IDBill);
            if (CheckProductInCart(cart))
            {
                //update product
                foreach (ProductInCartDetailsModel pro in cart.ProductsInCart)
                {
                    Product p = productRepo.GetProduct(pro.ProductID);
                    int newQuantity = p.Quantity - pro.Quantity;
                    p.Quantity = newQuantity;
                    if (newQuantity == 0)
                    {
                        p.StatusCode = "OUTS";
                    }
                    productRepo.UpdateOrderedProduct(p);
                }
                //update bill status ->payed
                billRepo.UpdateBillStatus(cart.BillID, "PAY");
                //send mail for user here
                Session.Remove("CartDetails");
                TempData["PaymentMessage"] = "Your bill (bill ID:" + cart.BillID + ") is ordered success!!";
                TempData["BillID"] = cart.BillID;

                string username = HttpContext.User.Identity.Name;
                User user = userRepo.GetUser(username);
                //send mail to user
                SendMailSSL.SendOrderedBillCODToUser(user.Email, cart, user.FullName);
            }
            else
            {
                TempData["ErrorOrder"] = "Error to order!!";
                return RedirectToAction("LoadCartDetails", "Cart");
            }
            return RedirectToAction("Index", "Product");
        }
    }
}