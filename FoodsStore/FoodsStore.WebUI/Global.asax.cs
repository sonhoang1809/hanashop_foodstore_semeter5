using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FoodsStore.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e) {

            bool sendEmail = true;
            var error = Server.GetLastError();
            string errorPage = "~/Error/Index?errorMessage=" + error.Message;

            List<string> recipients = new List<string>();
            recipients.Add("giatkt1598@gmail.com");
            recipients.Add("sonhoang1809@gmail.com");

            if (error is HttpException) {
                int code = (error as HttpException).GetHttpCode();
                if (code == 500) {
                    errorPage = "~/Error/InternalServerError";
                    //send email
                } else if (code == 404) {
                    sendEmail = false;
                    errorPage = "~/Error/PageNotFound";
                }
            }

            //send email
            if (sendEmail) {
                SmtpClient client = new SmtpClient {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("zyatkt1598@gmail.com", "1234@abcd"),
                    Port = 587,
                    Host = "smtp.gmail.com"
                };
                 foreach (string recipient in recipients) {
                    MailMessage mailMessage = new MailMessage(
                    from: "zyatkt1598@gmail.com",
                    to: recipient,
                    subject: "[HanaShop - Error] " + "Something wrong in your website",
                    body: error.ToString());
                    client.Send(mailMessage);
                }
            }

            Response.Clear();

            Server.ClearError();

            Response.Redirect(errorPage);
        }
    }
}
