using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace LGStarterKit.Contact
{
    /// <summary>
    ///  a simple surface controller for handling the contact forms.
    ///  
    ///  using a form based service is probibly better. 
    /// </summary>
    public class ContactFormSurfaceController : SurfaceController
    {
        /// <summary>
        ///  A Simple send , uses dictionary values to set from address 
        ///  and message template. 
        ///  
        ///  then does some simple substitution to put the message into
        ///  the template. 
        /// </summary>
        [HttpPost]
        public ActionResult SendContactEmail(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var from = Umbraco.GetDictionaryValue("Contact.From");
            var template =
                Umbraco.ReplaceLineBreaksForHtml(Umbraco.GetDictionaryValue("Contact.Message"));

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
            catch(Exception ex)
            {
                TempData.Add("ContactError", "Failed to Send Message: " + ex.ToString());
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