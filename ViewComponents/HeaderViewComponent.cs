using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using Site.Models.HomeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Site.Models.Navigation;

namespace Site.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public HeaderViewComponent(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public IViewComponentResult Invoke()
        {
            int websiteLanguageId = Int32.Parse(RouteData.Values["websiteLanguageId"].ToString());

            Dictionary<string, List<NavigationLinks>> NavigationDic = new Dictionary<string, List<NavigationLinks>>()
            {
                { "MainNav", new Models.Navigation(_context, _config).GetNavigationLinks(websiteLanguageId, "MainNav") }
            };

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                NavigationLinks = NavigationDic,
                WebsiteBundle = new Models.Website(_context, _config).GetWebsiteBundle(websiteLanguageId)
            };

            return View("_Header", DefaultViewModel);
        }
    }
}
