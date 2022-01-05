using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System;
using System.Threading.Tasks;
using System.Windows;
using BraintreeHttp;
namespace FoodsStore.WebUI.Paypal
{
    public class AuthorizeOrderSample
    {

        //This function can be used to perform authorization on the approved order.
        public async static Task<PayPalHttp.HttpResponse> AuthorizeOrder(string OrderId, bool debug = false)
        {
            var request = new OrdersAuthorizeRequest(OrderId);
            request.Prefer("return=representation");
            request.RequestBody(new AuthorizeRequest());
            var response = await PayPalClient.client().Execute(request);
            string s = "";
            if (debug)
            {
                var result = response.Result<Order>();
                s = s + "Status: " + result.Status + "\n";
                s = s + "Order Id: " + result.Id + "\n";
                s = s + "Authorization Id: "+ result.PurchaseUnits[0].Payments.Authorizations[0].Id + "\n";
                s = s + "Intent: " + result.CheckoutPaymentIntent +"\n";
                s = s + "Links:";
                foreach (LinkDescription link in result.Links)
                {
                    s = s + "Rel: " + link.Rel + " | " + link.Href + " | " + link.Method +"\n";
                }
                AmountWithBreakdown amount = result.PurchaseUnits[0].AmountWithBreakdown;
                s = s + "Buyer: ";
                s = s + "\tEmail Address: "+ result.Payer.Email;
               // Console.WriteLine("Response JSON: \n {0}", PayPalClient.ObjectToJSONString(result));
            }
            MessageBox.Show(s);
            return response;
        }
    }
}