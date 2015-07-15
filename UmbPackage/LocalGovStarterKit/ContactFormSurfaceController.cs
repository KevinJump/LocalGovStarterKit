using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Jumoo.LocalGov
{
    public class ContactFormSurfaceController : SurfaceController
    {
        [HttpPost]
        public ActionResult SendContact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var from = Umbraco.GetDictionaryValue("Contact.From");
            var template = Umbraco.GetDictionaryValue("Contact.Message");

            try
            {
                SmtpClient client = new SmtpClient();
                MailMessage msg = new MailMessage(from, model.ToAddress);

                msg.Body = template.Replace("{{name}}", model.Name)
                                .Replace("{{email}}", model.Email)
                                .Replace("{{message}}", model.Message);
                msg.IsBodyHtml = true;
                client.Send(msg);
            }
            catch( Exception ex)
            {
                TempData.Add("ContactError", "Failed to send message: " + ex.ToString());
            }

            return RedirectToCurrentUmbracoPage();
        }
    }

    public class ContactViewModel
    {
        public string ToAddress { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }
    }
}