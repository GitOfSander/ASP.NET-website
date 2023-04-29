using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Site.Data;
using System.Collections.Generic;
using System.Linq;
using static Site.Startup;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;
using Site.Models.App;
using static Site.Models.Page;

namespace Site.Models
{
    public class Routing : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Routing(SiteContext context = null, IOptions<AppSettings> config = null)
        {
            _context = context;
            _config = config;
        }

        public int Index;

        public Dictionary<string, object> ChildBreadcrumbs;

        public static Dictionary<string, object> ChildUrlParents;

        public class PageRoutes
        {
            public Pages Page { get; set; }
            public PageTemplates PageTemplate { get; set; }
            public IEnumerable<PageTemplateSections> PageTemplateSections { get; set; }
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public Languages Language { get; set; }
            public string FullUrl { get; set; }
        }

        public class DataRoutes
        {
            public DataTemplates DataTemplate { get; set; }
            public DataItems DataItem { get; set; }
            public IEnumerable<DataTemplateSections> DataTemplateSections { get; set; }
            public Pages Page { get; set; }
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public Languages Language { get; set; }
        }

        public class PageLanguages
        {
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public Languages Language { get; set; }
        }

        public class ParentBreadcrumbs
        {
            public List<Dictionary<string, object>> List = new List<Dictionary<string, object>>();
        }

        public class ParentUrlParents
        {
            public List<Dictionary<string, object>> List = new List<Dictionary<string, object>>();
        }

        public static void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                name: "default",
                template: "{controller= }/{action= }/{id:int}/{websiteLanguageId:int}/{defaultLanguage:bool}/{code= }/{culture= }/{timeZoneId= }/{parentUrlParents= }",
                defaults: new
                {
                    type = "spine-fallback"
                });

            var options = new DbContextOptionsBuilder<SiteContext>();
            options.UseSqlServer(ConnectionString);
            SiteContext _context = new SiteContext(options.Options);
            int WebsiteId = int.Parse(Startup.AppSettings.GetValue<string>("WebsiteId").ToString());

            List<PageRoutes> PageRoutes = _context.Pages.Join(_context.PageTemplates, Pages => Pages.PageTemplateId,
                                                              PageTemplates => PageTemplates.Id, (Pages, PageTemplates) => new { Pages, PageTemplates })
                                                         .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.PageTemplates, x.Pages, WebsiteLanguages })
                                                         .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.PageTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                                         .Where(x => x.WebsiteLanguages.WebsiteId == WebsiteId)
                                                         .Where(x => x.WebsiteLanguages.Active == true)
                                                         .Select(x => new PageRoutes()
                                                         {
                                                             Page = x.Pages,
                                                             PageTemplate = x.PageTemplates,
                                                             WebsiteLanguage = x.WebsiteLanguages,
                                                             Language = x.Languages
                                                         }).ToList();

            foreach (PageRoutes PageRoute in PageRoutes)
            {
                //Check if not default language. If not, then create a language url like: /nl/
                var defaultLanguageUrl = "";
                if (!PageRoute.WebsiteLanguage.DefaultLanguage)
                {
                    defaultLanguageUrl = PageRoute.Language.Code.ToLower() + "/";
                }

                if (PageRoute.Page.Parent == 0)
                {
                    routes.MapRoute(
                        name: PageRoute.Page.Id + "_" + PageRoute.Page.Url + "_" + PageRoute.PageTemplate.Controller + "_" + PageRoute.PageTemplate.Action,
                        template: defaultLanguageUrl + PageRoute.Page.Url,
                        defaults: new
                        {
                            id = PageRoute.Page.Id,
                            websiteLanguageId = PageRoute.WebsiteLanguage.Id,
                            defaultLanguage = PageRoute.WebsiteLanguage.DefaultLanguage,
                            code = PageRoute.Language.Code,
                            culture = PageRoute.Language.Culture,
                            timeZoneId = PageRoute.Language.TimeZoneId,
                            controller = PageRoute.PageTemplate.Controller,
                            action = PageRoute.PageTemplate.Action
                        },
                        constraints: null,
                        dataTokens: null);
                }
                else
                {
                    ParentUrlParents parentUrlParents = new ParentUrlParents { };
                    string Url = FindParentUrlAndCreateList(PageRoute.Page.Parent, PageRoute.Page.Url, PageRoutes, parentUrlParents).Remove(0, 1); //Remove first character because that is a / and that will give a error

                    routes.MapRoute(
                        name: PageRoute.Page.Id + "_" + PageRoute.Page.Url + "_" + PageRoute.PageTemplate.Controller + "_" + PageRoute.PageTemplate.Action,
                        template: defaultLanguageUrl + Url,
                        defaults: new
                        {
                            id = PageRoute.Page.Id,
                            websiteLanguageId = PageRoute.WebsiteLanguage.Id,
                            defaultLanguage = PageRoute.WebsiteLanguage.DefaultLanguage,
                            code = PageRoute.Language.Code,
                            culture = PageRoute.Language.Culture,
                            timeZoneId = PageRoute.Language.TimeZoneId,
                            controller = PageRoute.PageTemplate.Controller,
                            action = PageRoute.PageTemplate.Action
                        },
                        constraints: null,
                        dataTokens: new { parentUrlParents });
                }
            }

            List<DataRoutes> DataRoutes = _context.Pages.Join(_context.DataTemplates, Pages => Pages.AlternateGuid, DataTemplates => DataTemplates.PageAlternateGuid, (Pages, DataTemplates) => new { Pages, DataTemplates })
                                                        .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.DataTemplates, x.Pages, WebsiteLanguages })
                                                        .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.DataTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                                        .Where(x => x.DataTemplates.DetailPage == true)
                                                        .Where(x => x.WebsiteLanguages.WebsiteId == WebsiteId)
                                                        .Where(x => x.WebsiteLanguages.Active == true)
                                                        .Select(x => new DataRoutes()
                                                        {
                                                            DataTemplate = x.DataTemplates,
                                                            Page = x.Pages,
                                                            WebsiteLanguage = x.WebsiteLanguages,
                                                            Language = x.Languages
                                                        }).ToList();

            foreach (DataRoutes DataRoute in DataRoutes)
            {
                //Check if not default language. If not, then create a language url like: /nl/
                var DefaultLanguageUrl = "";
                if (!DataRoute.WebsiteLanguage.DefaultLanguage)
                {
                    DefaultLanguageUrl = DataRoute.Language.Code.ToLower() + "/";
                }

                if (DataRoute.Page.Parent == 0)
                {
                    routes.MapRoute(
                        name: DataRoute.Page.Id + "_" + DataRoute.Page.Url + "_" + DataRoute.DataTemplate.Controller + "_" + DataRoute.DataTemplate.Action,
                        template: DefaultLanguageUrl + DataRoute.Page.Url + "/{dataItemUrl}",
                        defaults: new
                        {
                            id = DataRoute.Page.Id,
                            parent = DataRoute.Page.Parent,
                            url = DataRoute.Page.Url,
                            name = DataRoute.Page.Name,
                            alternateGuid = DataRoute.Page.AlternateGuid,
                            dataTemplate = DataRoute.DataTemplate.CallName,
                            websiteLanguageId = DataRoute.WebsiteLanguage.Id,
                            defaultLanguage = DataRoute.WebsiteLanguage.DefaultLanguage,
                            code = DataRoute.Language.Code,
                            culture = DataRoute.Language.Culture,
                            timeZoneId = DataRoute.Language.TimeZoneId,
                            controller = DataRoute.DataTemplate.Controller,
                            action = DataRoute.DataTemplate.Action
                        },
                        constraints: null,
                        dataTokens: null);
                }
                else
                {
                    ParentUrlParents parentUrlParents = new ParentUrlParents { };
                    string Url = FindParentUrlAndCreateList(DataRoute.Page.Parent, DataRoute.Page.Url, PageRoutes, parentUrlParents).Remove(0, 1); //Remove first character because that is a / and that will give a error

                    routes.MapRoute(
                        name: DataRoute.Page.Id + "_" + DataRoute.Page.Url + "_" + DataRoute.DataTemplate.Controller + "_" + DataRoute.DataTemplate.Action,
                        template: DefaultLanguageUrl + Url + "/{dataItemUrl}",
                        defaults: new
                        {
                            id = DataRoute.Page.Id,
                            parent = DataRoute.Page.Parent,
                            url = DataRoute.Page.Url,
                            name = DataRoute.Page.Name,
                            alternateGuid = DataRoute.Page.AlternateGuid,
                            dataTemplate = DataRoute.DataTemplate.CallName,
                            websiteLanguageId = DataRoute.WebsiteLanguage.Id,
                            defaultLanguage = DataRoute.WebsiteLanguage.DefaultLanguage,
                            code = DataRoute.Language.Code,
                            culture = DataRoute.Language.Culture,
                            timeZoneId = DataRoute.Language.TimeZoneId,
                            controller = DataRoute.DataTemplate.Controller,
                            action = DataRoute.DataTemplate.Action
                        },
                        constraints: null,
                        dataTokens: new { parentUrlParents });
                }
            }

            routes.MapRoute(
                name: "fallback",
                template: "{*url:regex(^[^.]*$)}",
                defaults: new { controller = "Error", action = "RouteNotFoundAsync" });
        }

        public List<DataRoutes> GetDataRoutesByWebsiteLanguageIdAndDetailPage(int websiteLanguageId, bool detailPage)
        {
            return _context.Pages.Join(_context.DataTemplates.Where(DataTemplates => DataTemplates.DetailPage == detailPage), Pages => Pages.AlternateGuid, DataTemplates => DataTemplates.PageAlternateGuid, (Pages, DataTemplates) => new { Pages, DataTemplates })
                                 .Join(_context.DataItems, x => x.DataTemplates.Id, DataItems => DataItems.DataTemplateId, (x, DataItems) => new { x.DataTemplates, x.Pages, DataItems })
                                 .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.DataItems, x.DataTemplates, x.Pages, WebsiteLanguages })
                                 .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.DataItems, x.DataTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                 .GroupJoin(_context.DataTemplateSections, x => x.DataTemplates.Id, DataTemplateSections => DataTemplateSections.DataTemplateId, (x, DataTemplateSections) => new { x.DataTemplates, x.DataItems, x.Pages, x.WebsiteLanguages, x.Languages, DataTemplateSections })
                                 .Where(x => x.DataTemplates.DetailPage == true)
                                 .Where(x => x.WebsiteLanguages.Id == websiteLanguageId)
                                 .Where(x => x.WebsiteLanguages.Active == true)
                                 .Select(x => new DataRoutes()
                                 {
                                     DataItem = x.DataItems,
                                     DataTemplate = x.DataTemplates,
                                     DataTemplateSections = x.DataTemplateSections,
                                     Page = x.Pages,
                                     WebsiteLanguage = x.WebsiteLanguages,
                                     Language = x.Languages
                                 }).ToList();
        }

        public List<PageRoutes> GetPageRoutesByWebsiteLanguageId(int websiteLanguageId)
        {
            return _context.Pages.Join(_context.PageTemplates, Pages => Pages.PageTemplateId, PageTemplates => PageTemplates.Id, (Pages, PageTemplates) => new { Pages, PageTemplates })
                                 .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.PageTemplates, x.Pages, WebsiteLanguages })
                                 .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.PageTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                 .GroupJoin(_context.PageTemplateSections, x => x.PageTemplates.Id, PageTemplateSections => PageTemplateSections.PageTemplateId, (x, PageTemplateSections) => new { x.PageTemplates, x.Pages, x.WebsiteLanguages, x.Languages, PageTemplateSections })
                                 .Where(x => x.WebsiteLanguages.Id == websiteLanguageId)
                                 .Where(x => x.WebsiteLanguages.Active == true)
                                 .Select(x => new PageRoutes()
                                 {
                                     Page = x.Pages,
                                     PageTemplate = x.PageTemplates,
                                     PageTemplateSections = x.PageTemplateSections,
                                     WebsiteLanguage = x.WebsiteLanguages,
                                     Language = x.Languages
                                 }).ToList();
        }

        public string FindParentUrl(int Parent, string Url, List<PageRoutes> PageRoutes, bool DefaultLanguage, string code)
        {
            if (Parent != 0)
            {
                PageRoutes PageRoute = PageRoutes.Find(x => x.Page.Id == Parent);
                Url = PageRoute.Page.Url + "/" + Url;

                if (PageRoute.Page.Parent != 0)
                {
                    return Url = FindParentUrl(PageRoute.Page.Parent, Url, PageRoutes, DefaultLanguage, code);
                }
            }

            if (!DefaultLanguage)
            {
                return "/" + code.ToLower() + "/" + Url;
            }

            return "/" + Url;
        }

        private static string FindParentUrlAndCreateList(int Parent, string Url, List<PageRoutes> PageRoutes, ParentUrlParents ParentUrlParents)
        {
            PageRoutes PageRoute = PageRoutes.Find(x => x.Page.Id == Parent);
            Url = PageRoute.Page.Url + "/" + Url;

            ChildUrlParents = new Dictionary<string, object>()
            {
                { "Id", PageRoute.Page.Id },
                { "Parent", PageRoute.Page.Parent },
                { "Url", PageRoute.Page.Url }
            };
            ParentUrlParents.List.Add(ChildUrlParents);

            if (PageRoute.Page.Parent != 0)
            {
                return Url = FindParentUrlAndCreateList(PageRoute.Page.Parent, Url, PageRoutes, ParentUrlParents);
            }

            return "/" + Url;
        }

        public static string FindParentUrlInPageBundlesAndCreateList(int Parent, string Url, IQueryable<PageBundle> pageBundles, ParentUrlParents ParentUrlParents)
        {
            PageBundle _pageBundle = pageBundles.FirstOrDefault(x => x.Page.Id == Parent);
            Url = _pageBundle.Page.Url + "/" + Url;

            ChildUrlParents = new Dictionary<string, object>()
            {
                { "Id", _pageBundle.Page.Id },
                { "Parent", _pageBundle.Page.Parent },
                { "Url", _pageBundle.Page.Url }
            };
            ParentUrlParents.List.Add(ChildUrlParents);

            if (_pageBundle.Page.Parent != 0)
            {
                return Url = FindParentUrlInPageBundlesAndCreateList(_pageBundle.Page.Parent, Url, pageBundles, ParentUrlParents);
            }

            return "/" + Url;
        }

        private string FindParentUrlInDictionary(int parent, string url, ParentUrlParents ParentUrlParents)
        {
            //Check if the page has a parent
            if (parent != 0)
            {
                ChildUrlParents = ParentUrlParents.List.FirstOrDefault(x => Int32.Parse(x["Id"].ToString()) == parent);

                //Check if first time in this function then fill variable
                //if (FirstTimeInThisFunction) { Parent = (int)ChildUrlParents["Parent"]; }
                //FirstTimeInThisFunction = false;

                if (ChildUrlParents == null) { return url; }

                url = "/" + ChildUrlParents["Url"].ToString() + url;

                //Fire function again and return value
                return url = FindParentUrlInDictionary(Int32.Parse(ChildUrlParents["Parent"].ToString()), url, ParentUrlParents);
            }

            return url;
        }

        public ParentBreadcrumbs CreateBreadcrumb(int Parent, string Domain, ParentBreadcrumbs ParentBreadcrumbs, ParentUrlParents ParentUrlParents)
        {
            //Check if the page has a parent
            if (Parent != 0 && ParentUrlParents.List != null)
            {
                //Search for the parent
                //Dictionary<string, object> SearchPattern = new Dictionary<string, object>()
                //{
                //    { "Id", Parent }
                //};
                ChildUrlParents = ParentUrlParents.List.FirstOrDefault(x => Int32.Parse(x["Id"].ToString()) == Parent); //.FirstOrDefault(x => SearchPattern.All(x.Contains));

                //Does the function found a parent? no? then skips this and return before we get a eror
                if (ChildUrlParents != null)
                {
                    //Fill the parent variable
                    Parent = Int32.Parse(ChildUrlParents["Parent"].ToString());

                    ChildBreadcrumbs = new Dictionary<string, object>()
                    {
                        { "Title", ChildUrlParents["Url"].ToString().First().ToString().ToUpper() + string.Join("", ChildUrlParents["Url"].ToString().Skip(1)).Replace("-", " ") }
                    };

                    //Get url. After this function the ChildUrlParents has new data
                    var Url = FindParentUrlInDictionary(Int32.Parse(ChildUrlParents["Parent"].ToString()), "/" + (string)ChildUrlParents["Url"], ParentUrlParents);
                    ChildBreadcrumbs.Add("Url", Domain + Url);
                    ChildBreadcrumbs.Add("Position", --Index);
                    ParentBreadcrumbs.List.Add(ChildBreadcrumbs);

                    //Fire function again
                    ParentBreadcrumbs = CreateBreadcrumb(Parent, Domain, ParentBreadcrumbs, ParentUrlParents);
                }
            }

            return ParentBreadcrumbs;
        }

        public ParentBreadcrumbs CreateBreadcrumbs(RouteData RouteData, int websiteLanguageId, int parent, string url, string name, string dataItemTitle = null)
        {
            ParentBreadcrumbs ParentBreadcrumbs = new ParentBreadcrumbs();

            //Get website url
            string Domain = new Website(_context, _config).GetWebsiteUrl(websiteLanguageId);

            ParentUrlParents ParentUrlParents = null;

            object ObjectBreadcrumbs = RouteData.DataTokens.FirstOrDefault(x => x.Key == "parentUrlParents").Value;
            if (ObjectBreadcrumbs == null)
            {
                ObjectBreadcrumbs = RouteData.Values["parentUrlParents"];

                if (ObjectBreadcrumbs != null && ObjectBreadcrumbs.ToString() != "[]")
                {
                    ParentUrlParents = new ParentUrlParents() {
                        List = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ObjectBreadcrumbs.ToString()) as List<Dictionary<string, object>>
                    };

                    if (ParentUrlParents.List != null) {
                        ParentUrlParents.List.ToList();
                    }
                }
            }
            else if (ObjectBreadcrumbs != null)
            {
                ParentUrlParents = ObjectBreadcrumbs as ParentUrlParents;
                if (ParentUrlParents != null) { ParentUrlParents.List.ToList(); }
            }

            //ParentUrlParents = RouteData.DataTokens.FirstOrDefault(x => x.Key == "parentUrlParents").Value as ParentUrlParents;

            url = "/" + url;
            if (ParentUrlParents != null)
            {
                Index = 2 + ParentUrlParents.List.Count();
                url = FindParentUrlInDictionary(parent, url, ParentUrlParents);
            }
            else
            {
                Index = 2;
            }

            //Check if this is a detail page
            if (dataItemTitle != null)
            {
                ChildBreadcrumbs = new Dictionary<string, object>()
                {
                    { "Title", dataItemTitle.ToString().First().ToString().ToUpper() + string.Join("", dataItemTitle.ToString().Skip(1)) },
                    { "Url", Domain + url + "/" + RouteData.Values["dataItemUrl"] },
                    { "Position", ++Index }
                };
                ParentBreadcrumbs.List.Add(ChildBreadcrumbs);
            }

            //Add the url of the page you currently at to the dictionary 
            ChildBreadcrumbs = new Dictionary<string, object>()
            {
                { "Title", name.ToString().First().ToString().ToUpper() + string.Join("", name.ToString().Skip(1)) },
                { "Url", Domain + url },
                { "Position", --Index }
            };
            ParentBreadcrumbs.List.Add(ChildBreadcrumbs);

            if (ParentUrlParents != null)
            {
                //From now on we will look at the parent pages and create a breadcrumb for each parent
                ParentBreadcrumbs = CreateBreadcrumb(parent, Domain, ParentBreadcrumbs, ParentUrlParents);
            }

            //Get root page for the first breadcrumb
            Pages Page = new Page(_context, _config).GetRootPage(websiteLanguageId);

            ChildBreadcrumbs = new Dictionary<string, object>()
            {
                { "Title", Page.Name.ToString().First().ToString().ToUpper() + string.Join("", Page.Name.ToString().Skip(1)) },
                { "Url", Domain + "/" + Page.Url },
                { "Position", --Index } //Count 1 up, because this one isn't in the dictionary
            };
            ParentBreadcrumbs.List.Add(ChildBreadcrumbs);

            return ParentBreadcrumbs;
        }

        [Route("/spine-api/routes")]
        [HttpGet]
        public IActionResult GetRoutesApi()
        {
            try
            {
                List<PageRoutes> _pageRoutes = _context.Pages.Join(_context.PageTemplates, Pages => Pages.PageTemplateId, PageTemplates => PageTemplates.Id, (Pages, PageTemplates) => new { Pages, PageTemplates })
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

                foreach (PageRoutes pageRoute in _pageRoutes)
                {
                    string url = FindParentUrl(pageRoute.Page.Parent, pageRoute.Page.Url, _pageRoutes, pageRoute.WebsiteLanguage.DefaultLanguage, pageRoute.Language.Code);
                    pageRoute.FullUrl = url;
                }

                return Ok(Json(new
                {
                    pageRoutes = _pageRoutes
                }));
            }
            catch
            {
                return StatusCode(400, Json(new
                {
                    pageRoutes = ""
                }));
            }
        }
    }
}