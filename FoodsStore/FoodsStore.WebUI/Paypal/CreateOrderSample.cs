
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using FoodsStore.WebUI.Models;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace FoodsStore.WebUI.Paypal
{
    public class CreateOrderSample
    {
        private static List<Item> GetListItems(CartDetailsModel cart)
        {
            List<Item> result = new List<Item>();
            foreach (ProductInCartDetailsModel p in cart.ProductsInCart)
            {
                result.Add(
                    new Item
                    {
                        Name = p.ProductName,
                        Description = p.ShortDescription,
                        Sku = p.ProductID + "",
                        UnitAmount = new Money
                        {
                            CurrencyCode = "USD",
                            Value = p.Price + ""
                        },
                        Tax = new Money
                        {
                            CurrencyCode = "USD",
                            Value = (p.Price / 10) + ""
                        },
                        Quantity = p.Quantity + "",
                        Category = "PHYSICAL_GOODS"
                    }
                    );
            }
            return result;
        }
        
        //Below function can be used to build the create order request body with complete payload.
        public static OrderRequest BuildRequestBody(string fullname, string address, string street, string city, CartDetailsModel cart)
        {
            
            OrderRequest orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "AUTHORIZE",
                ApplicationContext = new ApplicationContext
                {
                    BrandName = "Hana Shop",
                    LandingPage = "BILLING",
                    CancelUrl = "https://localhost:44344/Cart/LoadCartDetails",
                    ReturnUrl = "https://localhost:44344/CheckOut/AuthorizeCaptureOrderPaypal",
                    UserAction = "CONTINUE",
                    ShippingPreference = "SET_PROVIDED_ADDRESS"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest{
                        ReferenceId =  "PUHF",
                        Description = "Sporting Goods",
                        CustomId = "CUST-HighFashions",
                        SoftDescriptor = "HighFashions",
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = cart.Total+"",
                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = cart.SubTotal+""
                                },
                                Shipping = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "00.00"
                                },
                                Handling = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "00.00"
                                },
                                TaxTotal = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = cart.Tax+""
                                },
                                ShippingDiscount = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "00.00"
                                }
                            }
                        },
                        Items = GetListItems(cart),
                        
                        ShippingDetail = new ShippingDetail
                        {
                            Name = new Name
                            {
                                FullName = fullname
                            },
                            AddressPortable = new AddressPortable
                            {
                                AddressLine1 = address,
                                AddressLine2 = street,
                                AdminArea2 = city,
                                AdminArea1 = "CA",
                                PostalCode = "94107",
                                CountryCode = "US"
                            }
                            
                        }
                    }
                }
            };

            return orderRequest;
        }

        //Below function can be used to create an order with complete payload.
        //public async static Task<HttpResponse> CreateOrder(bool debug = false)
        //{            
         //   var request = new OrdersCreateRequest();
         //   request.Prefer("return=representation");
         //   request.RequestBody(BuildRequestBody());
         //   var response = await PayPalClient.client().Execute(request);
         //       if (debug)
         //       {
         //           var result = response.Result<Order>();
         //           string s = "";
         //           s = s + "Status: " + result.Status + "\n";
         //           s = s + "Order Id: " + result.Id + "\n";
         //           s = s + "Intent: " + result.CheckoutPaymentIntent + "\n Links:";
         //           foreach (LinkDescription link in result.Links)
        //            {
        //                s = s + "Rel: " + link.Rel + " | " + link.Href + " | " + link.Method + "\n";
        //            }
        //            AmountWithBreakdown amount = result.PurchaseUnits[0].AmountWithBreakdown;
       //             s = s + "Total Amount: " + amount.CurrencyCode + amount.Value;
       //             //    Console.WriteLine("Response JSON: \n {0}", PayPalClient.ObjectToJSONString(result));
       //             MessageBox.Show(s);
        //        }            
        //    return response;

        //}
    }
}