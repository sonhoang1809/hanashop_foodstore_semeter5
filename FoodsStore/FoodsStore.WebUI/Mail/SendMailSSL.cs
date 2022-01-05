using FoodsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;


namespace FoodsStore.WebUI.Mail
{
    public class SendMailSSL
    {
        private static string GetHtmlOrderedBillWithPayPal(CartDetailsModel cart, string fullName)
        {
            string content = "<br><font>The payment has been authorized and approved.<br>" +
                "<br>Have a good day.<br>" +
                "From Hana Shop</font>";
            string messageBody = "<font>Dear " + fullName + ", <br><br>" +
                "Thank for your shopping !<br>" +
                "We have received $" + cart.Total + " that you submitted by PayPal at " + cart.LastTimeChange + ".<br></font>";
            string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
            string htmlTableEnd = "</table>";
            string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
            string htmlHeaderRowEnd = "</tr>";
            string htmlTrStart = "<tr style=\"color:#555555;\">";
            string htmlTrEnd = "</tr>";
            string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
            string htmlTdStartMerge = "<td colspan=\"4\" style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
            string htmlTdEnd = "</td>";
            messageBody += htmlTableStart;
            messageBody += htmlHeaderRowStart;
            messageBody += htmlTdStart + "STT" + htmlTdEnd;
            messageBody += htmlTdStart + "Product Name" + htmlTdEnd;
            messageBody += htmlTdStart + "Quantity" + htmlTdEnd;
            messageBody += htmlTdStart + "Unit Price" + htmlTdEnd;
            messageBody += htmlTdStart + "Total" + htmlTdEnd;
            messageBody += htmlHeaderRowEnd;
            int i = 1;
            foreach (ProductInCartDetailsModel p in cart.ProductsInCart)
            {
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + i + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + p.ProductName + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + p.Quantity + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "$" + p.Price + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "$" + p.Total + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                i++;
            }
            //show total, tax, subTotal
            messageBody = messageBody + htmlTrStart;
            messageBody = messageBody + htmlTdStartMerge + "Subtotal: " + htmlTdEnd;
            messageBody = messageBody + htmlTdStart + "$" + cart.SubTotal + htmlTdEnd + htmlTrEnd;
            messageBody = messageBody + htmlTrStart;
            messageBody = messageBody + htmlTdStartMerge + "Tax: " + htmlTdEnd;
            messageBody = messageBody + htmlTdStart + "$" + cart.Tax + htmlTdEnd + htmlTrEnd;
            messageBody = messageBody + htmlTrStart;
            messageBody = messageBody + htmlTdStartMerge + "Total: " + htmlTdEnd;
            messageBody = messageBody + htmlTdStart + "$" + cart.Total + htmlTdEnd + htmlTrEnd;

            //end
            messageBody = messageBody + htmlTableEnd;
            messageBody = messageBody + content;
            return messageBody;
        }
        private static string GetHtmlOrderedBillWithCOD(CartDetailsModel cart, string fullName)
        {
            string content = "<font><br>Have a good day.<br>" +
                "From Hana Shop</font>";
            string messageBody = "<font>Dear " + fullName + ", <br><br>" +
                "Thank for your shopping !<br>" +
                "We have received $" + cart.Total + " that you ordered at " + cart.LastTimeChange + ".<br></font>";
            string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
            string htmlTableEnd = "</table>";
            string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
            string htmlHeaderRowEnd = "</tr>";
            string htmlTrStart = "<tr style=\"color:#555555;\">";
            string htmlTrEnd = "</tr>";
            string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
            string htmlTdStartMerge = "<td colspan=\"4\" style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
            string htmlTdEnd = "</td>";
            messageBody += htmlTableStart;
            messageBody += htmlHeaderRowStart;
            messageBody += htmlTdStart + "STT" + htmlTdEnd;
            messageBody += htmlTdStart + "Product Name" + htmlTdEnd;
            messageBody += htmlTdStart + "Quantity" + htmlTdEnd;
            messageBody += htmlTdStart + "Unit Price" + htmlTdEnd;
            messageBody += htmlTdStart + "Total" + htmlTdEnd;
            messageBody += htmlHeaderRowEnd;
            int i = 1;
            foreach (ProductInCartDetailsModel p in cart.ProductsInCart)
            {
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + i + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + p.ProductName + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + p.Quantity + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "$" + p.Price + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "$" + p.Total + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                i++;
            }
            //show total, tax, subTotal
            messageBody = messageBody + htmlTrStart;
            messageBody = messageBody + htmlTdStartMerge + "Subtotal: " + htmlTdEnd;
            messageBody = messageBody + htmlTdStart + "$" + cart.SubTotal + htmlTdEnd + htmlTrEnd;
            messageBody = messageBody + htmlTrStart;
            messageBody = messageBody + htmlTdStartMerge + "Tax: " + htmlTdEnd;
            messageBody = messageBody + htmlTdStart + "$" + cart.Tax + htmlTdEnd + htmlTrEnd;
            messageBody = messageBody + htmlTrStart;
            messageBody = messageBody + htmlTdStartMerge + "Total: " + htmlTdEnd;
            messageBody = messageBody + htmlTdStart + "$" + cart.Total + htmlTdEnd + htmlTrEnd;

            //end
            messageBody = messageBody + htmlTableEnd;
            messageBody = messageBody + content;
            return messageBody;
        }
        public static void SendOrderedBillPayPalToUser(string mailReceive, CartDetailsModel cart, string fullname)
        {
            try
            {
                string content = GetHtmlOrderedBillWithPayPal(cart, fullname);               
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("sonsh1809@gmail.com", "sondep1809");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage message = new MailMessage();
                message.From = new MailAddress("sonsh1809@gmail.com", "Hana Shop");
                message.To.Add(new MailAddress(mailReceive));
                message.Subject = "Orders are ready to ship to you";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = content;
                smtp.Send(message);
            }
            catch (Exception) { }
        }
        public static void SendOrderedBillCODToUser(string mailReceive, CartDetailsModel cart, string fullname)
        {
            try
            {
                string content = GetHtmlOrderedBillWithCOD(cart, fullname);
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("sonsh1809@gmail.com", "sondep1809");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage message = new MailMessage();
                message.From = new MailAddress("sonsh1809@gmail.com", "Hana Shop");
                message.To.Add(new MailAddress(mailReceive));
                message.Subject = "Orders are ready to ship to you";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = content;
                smtp.Send(message);
            }
            catch (Exception) { }
        }

        public static void SendErrorToAdmin(string error)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com"; //for gmail host  
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("sonsh1809@gmail.com", "sondep1809");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage message = new MailMessage();
            message.From = new MailAddress("sonsh1809@gmail.com", "Hana Shop");
            message.To.Add(new MailAddress("sonhoang1809@gmail.com"));
            message.Subject = "Error !!";           
            message.Body = error;
            smtp.Send(message);
        }
    }
   
}
