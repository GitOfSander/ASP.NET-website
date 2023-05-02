using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using Site.Models.HomeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public FooterViewComponent(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public IViewComponentResult Invoke()
        {
            int websiteLanguageId = Int32.Parse(RouteData.Values["websiteLanguageId"].ToString());

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                WebsiteBundle = new Models.Website(_context, _config).GetWebsiteBundle(websiteLanguageId)
            };

            return View("_Footer", DefaultViewModel);
        }
    }
}
