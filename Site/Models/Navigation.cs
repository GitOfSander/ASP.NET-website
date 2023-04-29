using Site.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using static Site.Startup;
using Microsoft.EntityFrameworkCore.Internal;
using static Site.Models.Website;
using static Site.Models.Routing;
using Site.Models.App;

namespace Site.Models
{
    public class Navigation : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Navigation(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class NavigationBundle
        {
            public NavigationItems NavigationItem { get; set; }
            public Navigations Navigation { get; set; }
        }

        public class NavigationLinks
        {
            public string FullUrl { get; set; }
            public Pages Page { get; set; }
            public NavigationItems NavigationItem { get; set; }
        }

        public IEnumerable<NavigationBundle> GetNavigationBundlesByWebsiteLanguageIdAndCallName(int websiteLanguageId, string callName)
        {
            return _context.NavigationItems.Where(x => x.WebsiteLanguageId == websiteLanguageId)
                                           .Join(_context.Navigations.Where(x => x.CallName == callName), NavigationItems => NavigationItems.NavigationId, Navigations => Navigations.Id, (NavigationItems, Navigations) => new { NavigationItems, Navigations })
                                           .Select(x => new NavigationBundle()
                                           {
                                               NavigationItem = x.NavigationItems,
                                               Navigation = x.Navigations
                                           })
                                           .OrderBy(x => x.NavigationItem.CustomOrder)
                                           .ToList();
        }

        public List<NavigationLinks> GetNavigationLinks(int websiteLanguageId, string callName)
        {
            Routing routing = new Routing(_context);
            List<PageRoutes> _pageRoutes = routing.GetPageRoutesByWebsiteLanguageId(websiteLanguageId);
            List<DataRoutes> _dataRoutes = routing.GetDataRoutesByWebsiteLanguageIdAndDetailPage(websiteLanguageId, true);
            IEnumerable<NavigationBundle> _navigationBundles = new Navigation(_context, _config).GetNavigationBundlesByWebsiteLanguageIdAndCallName(websiteLanguageId, callName);

            List<NavigationLinks> navigationLinks = new List<NavigationLinks>();
            foreach (var i in _navigationBundles)
            {
                string url = "#";
                PageRoutes _pageRoute = new PageRoutes();
                Page page = new Page(_context, _config);

                switch (i.NavigationItem.LinkedToType.ToLower())
                {
                    case "page":
                        _pageRoute = _pageRoutes.Where(x => x.Page.AlternateGuid == i.NavigationItem.LinkedToAlternateGuid).FirstOrDefault();
                        url = page.GetPageUrl(_pageRoutes, _pageRoute, i) + ((i.NavigationItem.LinkedToSectionId != 0) ? "#" + page.GetSectionOrFilter(_pageRoute, i.NavigationItem) : ""); ;
                        break;
                    case "dataitem":
                        DataRoutes _dataRoute = _dataRoutes.FirstOrDefault(DataRoutes => DataRoutes.DataItem.AlternateGuid == i.NavigationItem.LinkedToAlternateGuid);
                        _pageRoute = _pageRoutes.Where(x => x.Page.AlternateGuid == _dataRoute.DataTemplate.PageAlternateGuid).FirstOrDefault();
                        url = page.GetPageUrl(_pageRoutes, _pageRoute, i) + "/" + _dataRoute.DataItem.PageUrl + ((i.NavigationItem.LinkedToSectionId != 0) ? "#" + new Data(_context).GetSectionOrFilter(_dataRoute, i.NavigationItem) : "");
                        break;
                    case "external":
                        url = i.NavigationItem.CustomUrl;
                        break;
                    default: // "nothing"
                        break;
                }

                // Something went wrong and the url couldn't be found. Cancel this operation and continue.
                if (url == null) { continue; };

                NavigationLinks navigationLink = new NavigationLinks()
                {
                    FullUrl = url,
                    Page = (_pageRoute != null) ? _pageRoute.Page : null,
                    NavigationItem = i.NavigationItem
                };
                navigationLinks.Add(navigationLink);
            }

            return navigationLinks;
        }

        [Route("/spine-api/navigation")]
        [HttpGet]
        public IActionResult GetNavigationLinksApi(int websiteLanguageId, string callName)
        {
            try
            {
                List<NavigationLinks> NavigationLinks = GetNavigationLinks(websiteLanguageId, callName);

                return Ok(Json(new
                {
                    navigationLinks = NavigationLinks
                }));
            }
            catch
            {
                return StatusCode(400, Json(new
                {
                    navigationLinks = ""
                }));
            }
        }
    }
}
