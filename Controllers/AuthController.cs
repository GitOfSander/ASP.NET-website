using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MimeKit;
using Org.BouncyCastle.Asn1.Ocsp;
using Site.Data;
using Site.Models;
using Site.Models.App;
using Site.Models.MailChimp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Site.Startup;

namespace Site.Controllers
{
    public class AuthController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;

        public AuthController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }
    }
}
