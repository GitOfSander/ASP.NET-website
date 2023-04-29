using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using Site.Models.MailChimp;
using Site.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Site.Models.MailChimp.Lists.Members;

namespace BaseTemplate.Controllers
{
    public class ApiController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;
        private readonly IEmailSender _emailSender;

        public ApiController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env, IEmailSender emailSender)
        {
            _context = context;
            _config = config;
            _env = env;
            _emailSender = emailSender;
        }

        public class MailInformation
        {
            public string name { get; set; } = "";
            public string email { get; set; } = "";
            public string phonenumber { get; set; } = "";
            public string message { get; set; } = "";
        }

        public class MailChimpResult
        {
            public string success { get; set; }
        }

        [Route("/spine-api/newspaper")]
        [HttpPost]
        public async Task<IActionResult> SendMailChimp([FromBody] MailInformation MailInformation)
        {
            //List<Dictionary<string, object>> PfParentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> Errors;
            Errors = new Dictionary<string, object>();
            //PfParentRow.Add(PfChildRow);

            ArrayList errors = new ArrayList();
            if (MailInformation.name == "")
            {
                Errors.Add("name", "Er is geen naam ingevuld.");
            }

            if (MailInformation.email != "")
            {
                if (!new EmailAddressAttribute().IsValid(MailInformation.email))
                {
                    Errors.Add("email", "Dit is geen geldig e-mailadres.");
                }
            }
            else
            {
                Errors.Add("email", "Er is geen e-mailadres ingevuld.");
            }

            if (Errors.Count > 0)
            {
                return Json(new
                {
                    success = false,
                    errors = Errors,
                });
            }

            bool result = await Task.Run(() => new MailChimpManager(_config.Value.OAuth.MailChimp.ApiKey, _config.Value.OAuth.MailChimp.ListId).AddMember("subscribed", MailInformation.email, MailInformation.name, ""));

            if (result)
            {
                return Json(new
                {
                    success = true,
                    result = "Hey " + MailInformation.name + ", u bent met succes toegevoegd aan onze nieuwsbrief!",
                });
            }
            else
            {
                Errors.Add("error", "U bent al aangemeld voor onze nieuwsbrief!");
                return Json(new
                {
                    success = false,
                    errors = Errors
                });
            }
        }

        [Route("/spine-api/unsubscribe-newspaper")]
        [HttpPost]
        public async Task<IActionResult> UbsubscribeMailChimp([FromBody] MailInformation MailInformation)
        {
            Dictionary<string, object> Errors;
            Errors = new Dictionary<string, object>();

            if (MailInformation.email != "")
            {
                if (!new EmailAddressAttribute().IsValid(MailInformation.email))
                {
                    Errors.Add("email", "Het ingevoerde e-mailadres is onjuist");
                }
            }
            else
            {
                Errors.Add("email", "Er is geen e-mailadres ingevuld.");
            }

            if (Errors.Count > 0)
            {
                return Json(new
                {
                    success = false,
                    errors = Errors,
                });
            }

            Member member = new Member()
            {
                EmailAddress = MailInformation.email
            };
            string hashed = member.GetSubscriberHash();

            var memberDel = new Member
            {
                EmailAddress = MailInformation.email.ToLower()
            }.GetSubscriberHash();

            if (await new MailChimpManager(_config.Value.OAuth.MailChimp.ApiKey, _config.Value.OAuth.MailChimp.ListId).DeleteMember(_config.Value.OAuth.MailChimp.ListId, memberDel))
            {
                return Json(new
                {
                    success = true,
                    result = "U bent succesvol uitgeschreven van onze nieuwsbrief",
                });
            }
            else
            {
                Errors.Add("error", "U bent al uitgeschreven van onze nieuwsbrief!");
                return Json(new
                {
                    success = false,
                    errors = Errors
                });
            }
        }

        [Route("/spine-api/contactform")]
        [HttpPost]
        public async Task<IActionResult> SendFormAsync([FromBody] MailInformation MailInformation)
        {
            //List<Dictionary<string, object>> PfParentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> Errors;
            Errors = new Dictionary<string, object>();
            //PfParentRow.Add(PfChildRow);

            ArrayList errors = new ArrayList();
            if (MailInformation.name == "")
            {
                Errors.Add("name", "Er is geen naam ingevuld.");
            }

            if (MailInformation.email != "")
            {
                if (!new EmailAddressAttribute().IsValid(MailInformation.email))
                {
                    Errors.Add("email", "Dit is geen geldig e-mailadres.");
                }
            }
            else
            {
                Errors.Add("email", "Er is geen e-mailadres ingevuld.");
            }

            if (MailInformation.phonenumber != "")
            {
                if (!new PhoneAttribute().IsValid(MailInformation.phonenumber))
                {
                    Errors.Add("phonenumber", "Dit is geen geldig telefoonnummer.");
                }
            }

            if (MailInformation.message == "")
            {
                Errors.Add("message", "Er is geen bericht ingevuld.");
            }

            if (Errors.Count > 0)
            {
                return Json(new
                {
                    success = false,
                    errors = Errors,
                });
            }

            string emailTemplateHtml = System.IO.File.ReadAllText(_env.WebRootPath + "\\spine-content\\templates\\email\\default\\index.html");
            emailTemplateHtml = emailTemplateHtml.Replace("<!panel:url!>", string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, "/spine-content/templates/email/default"));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<h1>" + _config.Value.Mail.Contact.Subject + "</h1><br />");
            sb.AppendLine("<table style='width: 100%;'><tbody>");
            sb.AppendLine("<tr><td valign='top'><b>Naam:&nbsp;&nbsp;</b></td><td>" + MailInformation.name + "</td></tr>");
            sb.AppendLine("<tr><td valign='top'><b>E-mailadres:&nbsp;&nbsp;</b></td><td>" + MailInformation.email + "</td></tr>");
            sb.AppendLine("<tr><td valign='top'><b>Telefoonnummer:&nbsp;&nbsp;</b></td><td>" + MailInformation.phonenumber + "</td></tr>");
            sb.AppendLine("<tr><td valign='top'><b>Bericht:&nbsp;&nbsp;</b></td><td>" + Regex.Replace(MailInformation.message, @"\r\n?|\n", "<br />") + "</td></tr>");
            sb.AppendLine("</tbody><table>");
            emailTemplateHtml = emailTemplateHtml.Replace("<!replace:body!>", sb.ToString());

            try
            {
                await _emailSender.SendEmailAsync(_config.Value.Mail.Contact.Email, _config.Value.Mail.Contact.Subject, emailTemplateHtml, _config.Value.Mail.Contact.FromName, _config.Value.Mail.Contact.FromEmail);
            }
            catch
            {
                Errors.Add("error", "Er is iets mis gegaan. Probeer het later opnieuw!");
                return Json(new
                {
                    success = false,
                    errors = Errors
                });
            }

            return Json(new
            {
                success = true,
                result = "Hey " + MailInformation.name + ", uw bericht is succesvol verzonden!",
            });
        }
    }
}
