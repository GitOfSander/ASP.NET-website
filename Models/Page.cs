using Site.Data;
using Site.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using static Site.Models.Website;
using static Site.Models.Routing;
using Site.Models.App;
using Microsoft.Extensions.Options;
using static Site.Models.Navigation;
using System.Text.RegularExpressions;

namespace Site.Models
{
    public class Page : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Page(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class PageBundle
        {
            public PageFiles PageFile { get; set; }
            public IEnumerable<PageFiles> PageFiles { get; set; }
            public PageResources PageResource { get; set; }
            public IEnumerable<PageResources> PageResources { get; set; }
            public Pages Page { get; set; }
            public PageTemplateFields PageTemplateField { get; set; }
            public IEnumerable<PageTemplateFields> PageTemplateFields { get; set; }
            public PageTemplates PageTemplate { get; set; }
            public PageTemplateUploads PageTemplateUpload { get; set; }
            public IEnumerable<PageTemplateUploads> PageTemplateUploads { get; set; }
        }

        private List<PageRoutes> _pageRoutes;
        private WebsiteBundle _websiteBundle;

        public PageBundle GetPageBundle(int pageId)
        {
            return _context.PageTemplates.Join(_context.Pages, PageTemplates => PageTemplates.Id, Pages => Pages.PageTemplateId, (PageTemplates, Pages) => new { PageTemplates, Pages })
                                         .GroupJoin(_context.PageFiles.Where(PageFiles => PageFiles.Active == true)
                                                                      .Join(_context.PageTemplateUploads, PageFiles => PageFiles.PageTemplateUploadId, PageTemplateUploads => PageTemplateUploads.Id, (PageFiles, PageTemplateUploads) => new { PageFiles, PageTemplateUploads })
                                                                      .OrderBy(x => x.PageFiles.CustomOrder),
                                         x => x.Pages.Id, x => x.PageFiles.PageId, (x, PageFiles) => new { x.Pages, x.PageTemplates, PageFiles })
                                         .GroupJoin(_context.PageResources.Join(_context.PageTemplateFields, PageResources => PageResources.PageTemplateFieldId, PageTemplateFields => PageTemplateFields.Id, (PageResources, PageTemplateFields) => new { PageResources, PageTemplateFields }), x => x.Pages.Id, x => x.PageResources.PageId, (x, PageResources) => new { x.Pages, x.PageTemplates, x.PageFiles, PageResources })
                                         .Where(x => x.Pages.Active == true)
                                         .Where(x => x.Pages.Id == pageId)
                                         .Select(x => new PageBundle()
                                         {
                                             PageFiles = x.PageFiles.Select(y => y.PageFiles),
                                             PageResources = x.PageResources.Select(y => y.PageResources),
                                             Page = x.Pages,
                                             PageTemplateFields = x.PageResources.Select(y => y.PageTemplateFields),
                                             PageTemplate = x.PageTemplates,
                                             PageTemplateUploads = x.PageFiles.Select(y => y.PageTemplateUploads),
                                         })
                                         .FirstOrDefault();
        }

        public Pages GetRootPage(int WebsiteLanguageId)
        {
            return _context.WebsiteLanguages.Join(_context.Websites, WebsiteLanguages => WebsiteLanguages.WebsiteId, Websites => Websites.Id, (WebsiteLanguages, Websites) => new { WebsiteLanguages, Websites })
                                            .Join(_context.Pages, x => x.WebsiteLanguages.Id, Pages => Pages.WebsiteLanguageId, (x, Pages) => new { x.WebsiteLanguages, x.Websites, Pages })
                                            .Where(x => x.Websites.RootPageAlternateGuid == x.Pages.AlternateGuid)
                                            .Select(x => x.Pages)
                                            .FirstOrDefault();
        }

        public Pages GetPage(int WebsiteLanguageId, string AlternateGuid)
        {
            Pages Page = _context.Pages.Where(Pages => Pages.AlternateGuid == AlternateGuid).FirstOrDefault(Pages => Pages.WebsiteLanguageId == WebsiteLanguageId);

            return Page;
        }

        public bool SetBundlesForUrlFunction(int WebsiteLanguageId)
        {
            _pageRoutes = _context.Pages.Join(_context.PageTemplates, Pages => Pages.PageTemplateId, PageTemplates => PageTemplates.Id, (Pages, PageTemplates) => new { Pages, PageTemplates })
                                        .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.PageTemplates, x.Pages, WebsiteLanguages })
                                        .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.PageTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                        .Where(x => x.WebsiteLanguages.Id == WebsiteLanguageId)
                                        .Select(x => new PageRoutes()
                                        {
                                            Page = x.Pages,
                                            PageTemplate= x.PageTemplates,
                                            WebsiteLanguage = x.WebsiteLanguages,
                                            Language = x.Languages
                                        }).ToList();

            _websiteBundle = _context.WebsiteLanguages.Join(_context.Languages, WebsiteLanguages => WebsiteLanguages.LanguageId, Languages => Languages.Id, (WebsiteLanguages, Languages) => new { WebsiteLanguages, Languages })
                                                      .Where(x => x.WebsiteLanguages.Id == WebsiteLanguageId)
                                                      .Select(x => new WebsiteBundle()
                                                      {
                                                          Language = x.Languages,
                                                          WebsiteLanguage = x.WebsiteLanguages
                                                      }).FirstOrDefault();

            if (_pageRoutes != null && _websiteBundle != null) { return true; }

            return false;
        }

        public string GetPageUrlByAlternateGuid(string AlternateGuid)
        {
            Pages _page = _pageRoutes.Where(x => x.Page.AlternateGuid == AlternateGuid).Select(x => x.Page).FirstOrDefault();

            string Url = "";
            if (_page != null)
            {
                Url = new Routing(_context).FindParentUrl(_page.Parent, _page.Url, _pageRoutes, _websiteBundle.WebsiteLanguage.DefaultLanguage, _websiteBundle.Language.Code);
            }

            return Url;
        }

        public List<PageRoutes> GetPageRoutes()
        {
            return _context.Pages.Join(_context.PageTemplates, Pages => Pages.PageTemplateId, PageTemplates => PageTemplates.Id, (Pages, PageTemplates) => new { Pages, PageTemplates })
                                 .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.PageTemplates, x.Pages, WebsiteLanguages })
                                 .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.PageTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                 .Where(x => x.WebsiteLanguages.WebsiteId == _config.Value.WebsiteId)
                                 .Where(x => x.WebsiteLanguages.Active == true)
                                 .Select(x => new PageRoutes()
                                 {
                                     Page = x.Pages,
                                     PageTemplate = x.PageTemplates,
                                     WebsiteLanguage = x.WebsiteLanguages,
                                     Language = x.Languages
                                 }).ToList();
        }

        public string GetPageUrl(List<PageRoutes> pageRoutes, PageRoutes pageRoute, NavigationBundle navigationBundle)
        {
            if (pageRoute != null)
            {
                if (pageRoute.Page != null)
                {
                    return new Routing(_context, _config).FindParentUrl(pageRoute.Page.Parent, pageRoute.Page.Url, pageRoutes, pageRoute.WebsiteLanguage.DefaultLanguage, pageRoute.Language.Code);
                }
            }

            return null;
        }

        public string GetSectionOrFilter(PageRoutes pageRoute, NavigationItems navigationItem)
        {
            PageTemplateSections pageTemplateSection = pageRoute.PageTemplateSections.FirstOrDefault(PageTemplateSections => PageTemplateSections.Id == navigationItem.LinkedToSectionId);
            switch (pageTemplateSection.Type.ToLower())
            {
                case "section":
                    return pageTemplateSection.Section;
                default: // "datafilter"
                    return Regex.Replace(_context.DataItems.FirstOrDefault(DataItems => DataItems.AlternateGuid == navigationItem.FilterAlternateGuid).AlternateGuid, @"[^A-Za-z0-9_\.~]+", "-");
            }
        }

        public PageBundle ChangeTextByType(PageBundle pageBundle)
        {
            foreach (PageResources pageResource in pageBundle.PageResources)
            {
                string type = pageBundle.PageTemplateFields.FirstOrDefault(PageTemplateFields => PageTemplateFields.Id == pageResource.PageTemplateFieldId).Type;
                string text = pageResource.Text;
                if (type.ToLower() == "textarea")
                {
                    pageBundle.PageResources.FirstOrDefault(PageResources => PageResources.Id == pageResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                }
            }

            return pageBundle;
        }

        [Route("/spine-api/page")]
        [HttpGet]
        public IActionResult GetPageBundleApi(int pageId)
        {
            try
            {
                string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

                PageBundle _pageBundle = GetPageBundle(pageId);

                List<Dictionary<string, object>> pageFilesParentRow = new List<Dictionary<string, object>>();
                Dictionary<string, object> pageFilesChildRow;
                foreach (PageFiles pageFile in _pageBundle.PageFiles)
                {
                    if (pageFile.Active)
                    {
                        pageFilesChildRow = new Dictionary<string, object>()
                        {
                            { "callName", _pageBundle.PageTemplateUploads.FirstOrDefault(PageTemplateUploads => PageTemplateUploads.Id == pageFile.PageTemplateUploadId).CallName},
                            { "originalPath", url + pageFile.OriginalPath.Replace("~/", "/")},
                            { "compressedPath", url + pageFile.CompressedPath.Replace("~/", "/")},
                            { "alt", pageFile.Alt},
                            { "customOrder", pageFile.CustomOrder}
                        };
                        pageFilesParentRow.Add(pageFilesChildRow);
                    }
                }

                List<Dictionary<string, object>> pageResourcesParentRow = new List<Dictionary<string, object>>();
                Dictionary<string, object> pageResourcesChildRow;
                foreach (PageResources pageResource in _pageBundle.PageResources)
                {
                    string type = _pageBundle.PageTemplateFields.FirstOrDefault(PageTemplateFields => PageTemplateFields.Id == pageResource.PageTemplateFieldId).Type;
                    string text = pageResource.Text;
                    if (type.ToLower() == "textarea")
                    {
                        text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }
                    pageResourcesChildRow = new Dictionary<string, object>()
                    {
                        { "callName", _pageBundle.PageTemplateFields.FirstOrDefault(PageTemplateFields => PageTemplateFields.Id == pageResource.PageTemplateFieldId).CallName},
                        { "text", text}
                    };
                    pageResourcesParentRow.Add(pageResourcesChildRow);
                }

                return Ok(Json(new
                {
                    page = _pageBundle.Page,
                    pageFiles = pageFilesParentRow,
                    pageResources = pageResourcesParentRow
                }));
            }
            catch
            {
                return StatusCode(400, Json(new
                {
                    page = "",
                    pageFilesParentRow = "",
                    pageResources = ""
                }));
            }
        }
    }
}
