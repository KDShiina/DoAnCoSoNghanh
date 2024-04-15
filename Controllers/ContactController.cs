using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSoNghanh.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendEmail(string name, string email, string subject, string message)
        {
            try
            {
                var toAddress = "quoccthinhh03@gmail.com";
                var fromAddress = new MailAddress("quocthinh5.v2003@gmail.com", "Khách Hàng"); // Replace with your email and name
                const string fromPassword = "uuiv yswn atcy yomu"; // Replace with your email password
                string body = $"Name: {name}\nEmail: {email}\nSubject: {subject}\n\nMessage:\n{message}";

                var smtp = new SmtpClient
                {
                    Host = "quocthinh5.v2003@gmail.com", // Replace with your SMTP server
                    Port = 587, // Replace with your SMTP port
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var mailMessage = new MailMessage(fromAddress, new MailAddress(toAddress))
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(mailMessage);
                }

                ViewBag.Message = "Email sent successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
            }

            return View("Index");
        }
    }
}