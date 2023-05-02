using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using Site.Models.HomeViewModels;
using System;
using System.Collections.Generic;
using static Site.Startup;
using static Site.Models.Data;
using static Site.Models.Page;
using System.Linq;

namespace Site.Controllers
{
    public class HomeController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public HomeController(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public IActionResult Index()
        {
            int id = Int32.Parse(RouteData.Values["id"].ToString());
            Page page = new Page(_context, _config);
            PageBundle _pageBundle = page.ChangeTextByType(page.GetPageBundle(id));

            ViewBag.PageTitle = _pageBundle.Page.Title;
            ViewBag.PageKeywords = _pageBundle.Page.Keywords;
            ViewBag.PageDescription = _pageBundle.Page.Description;

            int websiteLanguageId = _pageBundle.Page.WebsiteLanguageId;

            Models.Data data = new Models.Data(_context, _config);
            Dictionary<string, IEnumerable<DataBundle>> DataDic = new Dictionary<string, IEnumerable<DataBundle>>()
            {
                { "Projects", data.ChangeDataBundlesTextByType(data.GetDataItemsWithCategorie(websiteLanguageId, "Projects")) },
                { "Slider",  data.ChangeDataBundlesTextByType(data.GetDataBundles(websiteLanguageId, "Slider", true, true)) }
            };

            if (page.SetBundlesForUrlFunction(websiteLanguageId))
            {
                string AboutUrl = page.GetPageUrlByAlternateGuid("d8f3e656-92a3-463f-b0f3-bdbf6ae34e7f");
                string EducationUrl = page.GetPageUrlByAlternateGuid("3287e60f-c8c3-4c83-958e-da705a5ed997");
                string MaintenanceUrl = page.GetPageUrlByAlternateGuid("16b42a99-2d7f-45ea-a2c7-d9ef9ca407b2");
                string ProjectsUrl = page.GetPageUrlByAlternateGuid("67ed7381-8542-4269-ac85-925e07c5aa8e");
                Dictionary<string, string> Links = new Dictionary<string, string>()
                {
                    { "AboutUrl", AboutUrl },
                    { "EducationUrl", EducationUrl },
                    { "MaintenanceUrl", MaintenanceUrl },
                    { "ProjectsUrl", ProjectsUrl }
                };

                DefaultViewModel DefaultViewModel = new DefaultViewModel()
                {
                    DataBundles = DataDic,
                    Links = Links,
                    PageBundle = _pageBundle
                };

                return View(DefaultViewModel);
            }

            return Redirect("/404");
        }

        public IActionResult Contact()
        {
            int id = Int32.Parse(RouteData.Values["id"].ToString());
            Page page = new Page(_context, _config);
            PageBundle _pageBundle = page.ChangeTextByType(page.GetPageBundle(id));

            ViewBag.PageTitle = _pageBundle.Page.Title;
            ViewBag.PageKeywords = _pageBundle.Page.Keywords;
            ViewBag.PageDescription = _pageBundle.Page.Description;

            int websiteLanguageId = _pageBundle.Page.WebsiteLanguageId;

            Routing.ParentBreadcrumbs parentBreadcrumbs = new Routing(_context, _config).CreateBreadcrumbs(RouteData, websiteLanguageId, _pageBundle.Page.Parent, _pageBundle.Page.Url, _pageBundle.Page.Name);

            Website website = new Website(_context, _config);

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                Breadcrumbs = parentBreadcrumbs,
                PageBundle = _pageBundle,
                WebsiteBundle = website.ChangeTextByType(website.GetWebsiteBundle(websiteLanguageId))
            };

            return View(DefaultViewModel);
        }

        public IActionResult Projects()
        {
            int id = Int32.Parse(RouteData.Values["id"].ToString());
            Page page = new Page(_context, _config);
            PageBundle _pageBundle = page.ChangeTextByType(page.GetPageBundle(id));

            ViewBag.PageTitle = _pageBundle.Page.Title;
            ViewBag.PageKeywords = _pageBundle.Page.Keywords;
            ViewBag.PageDescription = _pageBundle.Page.Description;

            int websiteLanguageId = _pageBundle.Page.WebsiteLanguageId;

            Models.Data data = new Models.Data(_context, _config);
            IQueryable<DataBundle> _dataBundles = data.ChangeDataBundlesTextByType(data.GetDataItemsWithCategorie(websiteLanguageId, "Projects"));
            string pageAlternateGuid = _dataBundles.FirstOrDefault().DataTemplate.PageAlternateGuid;

            Dictionary<string, IEnumerable<DataBundle>> dataDic = new Dictionary<string, IEnumerable<DataBundle>>()
            {
                { "Projects", _dataBundles },
                { "ProjectCategories", data.ChangeDataBundlesTextByType(data.GetDataBundles(websiteLanguageId, "ProjectCategories", false, false)) }
            };

            string Url = new Models.Data(_context, _config).GetUrlBeforeDataItemByPageAlternateGuid(websiteLanguageId, pageAlternateGuid);

            Dictionary<string, string> Links = new Dictionary<string, string>()
            {
                { "ProjectsUrl", Url }
            };

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                DataBundles = dataDic,
                Links = Links
            };

            return View(DefaultViewModel);
        }

        public IActionResult Project()
        {
            string DataItemUrl = (string)RouteData.Values["dataItemUrl"];
            string DataTemplate = (string)RouteData.Values["dataTemplate"];
            int websiteLanguageId = (int)RouteData.Values["websiteLanguageId"];

            Models.Data data = new Models.Data(_context, _config);
            string url = data.GetUrlBeforeDataItemByPageAlternateGuid(websiteLanguageId, (string)RouteData.Values["AlternateGuid"]);
            DataBundle DataBundle = data.ChangeDataBundleTextByType(data.GetDataItemWithCategorie(websiteLanguageId, DataTemplate, DataItemUrl));

            if (DataBundle != null)
            {
                ViewBag.PageTitle = (string)DataBundle.DataItem.PageTitle;
                ViewBag.PageKeywords = (string)DataBundle.DataItem.PageKeywords;
                ViewBag.PageDescription = (string)DataBundle.DataItem.PageDescription;

                Dictionary<string, DataBundle> DataItemDic = new Dictionary<string, DataBundle>()
                {
                    { "Project", DataBundle }
                };

                Dictionary<string, string> Links = new Dictionary<string, string>()
                {
                    { "ProjectsUrl", url }
                };

                DataBundle PreviousDataItem = new Models.Data(_context, _config).GetPreviousDataItem(websiteLanguageId, DataTemplate, DataBundle.DataItem.CustomOrder);
                if (PreviousDataItem != null)
                {
                    Links.Add("PreviousProject", PreviousDataItem.DataItem.PageUrl.ToString());
                }
                DataBundle NextDataItem = new Models.Data(_context, _config).GetNextDataItem(websiteLanguageId, DataTemplate, DataBundle.DataItem.CustomOrder);
                if (NextDataItem != null)
                {
                    Links.Add("NextProject", NextDataItem.DataItem.PageUrl.ToString());
                }

                Routing.ParentBreadcrumbs parentBreadcrumbs = new Routing(_context, _config).CreateBreadcrumbs(RouteData, websiteLanguageId, (int)RouteData.Values["parent"], (string)RouteData.Values["url"], (string)RouteData.Values["name"], DataBundle.DataItem.Title);

                DefaultViewModel DefaultViewModel = new DefaultViewModel()
                {
                    Breadcrumbs = parentBreadcrumbs,
                    DataBundle = DataItemDic,
                    Links = Links
                };

                return View(DefaultViewModel);
            }
            else
            {
                return Redirect(url);
            }
        }

        public IActionResult InformationText()
        {
            int id = Int32.Parse(RouteData.Values["id"].ToString());
            Page page = new Page(_context, _config);
            PageBundle _pageBundle = page.ChangeTextByType(page.GetPageBundle(id));

            ViewBag.PageTitle = _pageBundle.Page.Title;
            ViewBag.PageKeywords = _pageBundle.Page.Keywords;
            ViewBag.PageDescription = _pageBundle.Page.Description;

            int websiteLanguageId = _pageBundle.Page.WebsiteLanguageId;

            Routing.ParentBreadcrumbs parentBreadcrumbs = new Routing(_context, _config).CreateBreadcrumbs(RouteData, websiteLanguageId, _pageBundle.Page.Parent, _pageBundle.Page.Url, _pageBundle.Page.Name);

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                Breadcrumbs = parentBreadcrumbs,
                PageBundle = _pageBundle
            };

            return View(DefaultViewModel);
        }

        public IActionResult InformationImageText()
        {
            int id = Int32.Parse(RouteData.Values["id"].ToString());
            Page page = new Page(_context, _config);
            PageBundle _pageBundle = page.ChangeTextByType(page.GetPageBundle(id));

            ViewBag.PageTitle = _pageBundle.Page.Title;
            ViewBag.PageKeywords = _pageBundle.Page.Keywords;
            ViewBag.PageDescription = _pageBundle.Page.Description;

            int websiteLanguageId = _pageBundle.Page.WebsiteLanguageId;

            Routing.ParentBreadcrumbs parentBreadcrumbs = new Routing(_context, _config).CreateBreadcrumbs(RouteData, websiteLanguageId, _pageBundle.Page.Parent, _pageBundle.Page.Url, _pageBundle.Page.Name);

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                Breadcrumbs = parentBreadcrumbs,
                PageBundle = _pageBundle
            };

            return View(DefaultViewModel);
        }

        public IActionResult About()
        {
            int id = Int32.Parse(RouteData.Values["id"].ToString());
            Page page = new Page(_context, _config);
            PageBundle _pageBundle = page.ChangeTextByType(page.GetPageBundle(id));

            ViewBag.PageTitle = _pageBundle.Page.Title;
            ViewBag.PageKeywords = _pageBundle.Page.Keywords;
            ViewBag.PageDescription = _pageBundle.Page.Description;

            int websiteLanguageId = _pageBundle.Page.WebsiteLanguageId;

            Models.Data data = new Models.Data(_context, _config);
            Dictionary<string, IEnumerable<DataBundle>> DataDic = new Dictionary<string, IEnumerable<DataBundle>>()
            {
                { "QualityMarks", data.ChangeDataBundlesTextByType(data.GetDataBundles(websiteLanguageId, "QualityMarks", true, true)) }
            };

            Routing.ParentBreadcrumbs parentBreadcrumbs = new Routing(_context, _config).CreateBreadcrumbs(RouteData, websiteLanguageId, _pageBundle.Page.Parent, _pageBundle.Page.Url, _pageBundle.Page.Name);

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                Breadcrumbs = parentBreadcrumbs,
                DataBundles = DataDic,
                PageBundle = _pageBundle
            };

            return View(DefaultViewModel);
        }

        public IActionResult Team()
        {
            int id = Int32.Parse(RouteData.Values["id"].ToString());
            Page page = new Page(_context, _config);
            PageBundle _pageBundle = page.ChangeTextByType(page.GetPageBundle(id));

            ViewBag.PageTitle = _pageBundle.Page.Title;
            ViewBag.PageKeywords = _pageBundle.Page.Keywords;
            ViewBag.PageDescription = _pageBundle.Page.Description;

            int websiteLanguageId = _pageBundle.Page.WebsiteLanguageId;

            Models.Data data = new Models.Data(_context, _config);
            Dictionary<string, IEnumerable<DataBundle>> DataDic = new Dictionary<string, IEnumerable<DataBundle>>()
            {
                { "Employees", data.ChangeDataBundlesTextByType(data.GetDataBundles(websiteLanguageId, "Employees", true, true)) }
            };

            Routing.ParentBreadcrumbs parentBreadcrumbs = new Routing(_context, _config).CreateBreadcrumbs(RouteData, websiteLanguageId, _pageBundle.Page.Parent, _pageBundle.Page.Url, _pageBundle.Page.Name);

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                Breadcrumbs = parentBreadcrumbs,
                DataBundles = DataDic,
                PageBundle = _pageBundle
            };

            return View(DefaultViewModel);
        }

        public IActionResult Sponsor()
        {
            int id = Int32.Parse(RouteData.Values["id"].ToString());
            Page page = new Page(_context, _config);
            PageBundle _pageBundle = page.ChangeTextByType(page.GetPageBundle(id));

            ViewBag.PageTitle = _pageBundle.Page.Title;
            ViewBag.PageKeywords = _pageBundle.Page.Keywords;
            ViewBag.PageDescription = _pageBundle.Page.Description;

            int websiteLanguageId = _pageBundle.Page.WebsiteLanguageId;

            Models.Data data = new Models.Data(_context, _config);
            Dictionary<string, IEnumerable<DataBundle>> DataDic = new Dictionary<string, IEnumerable<DataBundle>>()
            {
                { "Sponsorships", data.ChangeDataBundlesTextByType(data.GetDataBundles(websiteLanguageId,  "Sponsorships", true, true)) }
            };

            Routing.ParentBreadcrumbs parentBreadcrumbs = new Routing(_context, _config).CreateBreadcrumbs(RouteData, websiteLanguageId, _pageBundle.Page.Parent, _pageBundle.Page.Url, _pageBundle.Page.Name);

            DefaultViewModel DefaultViewModel = new DefaultViewModel()
            {
                Breadcrumbs = parentBreadcrumbs,
                DataBundles = DataDic,
                PageBundle = _pageBundle
            };

            return View(DefaultViewModel);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
