using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using Site.Data;
using Site.Models;
using Site.Models.App;
using Site.Models.HomeViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using static Site.Models.Data;
using static Site.Models.Language;
using static Site.Models.Navigation;
using static Site.Models.Page;
using static Site.Models.Review;

namespace Site.Controllers
{
    public class ErrorController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public ErrorController(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public async Task<IActionResult> RouteNotFoundAsync(string url)
        {
            string[] urlSplit = url.Split('/');
            List<string> urlSplitList = new List<string>(urlSplit);

            LanguagesBundle _languageBundle = null;

            //Get list with active languages this website has
            IQueryable<LanguagesBundle> _languagesBundles = new Language(_context, _config).GetActiveLanguages(_config.Value.WebsiteId);
            if (_languagesBundles.Count() > 1)
            {
                string languageCode = urlSplitList.First();
                //Check if it's possible that it is a language code
                if (languageCode.Length == 2)
                {
                    //If it is, the function should find one in the list
                    _languageBundle = _languagesBundles.FirstOrDefault(x => x.Language.Code == languageCode.ToLower());
                }
            }

            //If there isn't a language code in the url then take the default language
            if (_languageBundle == null)
            {
                _languageBundle = _languagesBundles.FirstOrDefault(x => x.WebsiteLanguage.DefaultLanguage == true);
            }
            else
            {
                //Remove first url segment, because that is a language
                urlSplitList.RemoveAt(0);

                //Check if the language is the default. If it is, the url wouldn't be allowed and we send the user to the 404 page
                if (_languageBundle.WebsiteLanguage.DefaultLanguage)
                {
                    return RedirectToRoute("404");
                }
            }

            if (_languageBundle != null)
            {
                //Get all active pages by language
                IQueryable<PageBundle> _pageBundles = _context.Pages.Join(_context.PageTemplates, Pages => Pages.PageTemplateId, PageTemplates => PageTemplates.Id, (Pages, PageTemplates) => new { Pages, PageTemplates })
                                                                    .Where(x => x.Pages.WebsiteLanguageId == _languageBundle.WebsiteLanguage.Id).Where(x => x.Pages.Active == true)
                                                                    .Select(x => new PageBundle()
                                                                    {
                                                                        Page = x.Pages,
                                                                        PageTemplate = x.PageTemplates
                                                                    });

                //Get last segment from the url
                string lastUrlSegment = urlSplitList.Last();

                //Get list with pages that match the last segment
                IQueryable<PageBundle> _pageBundlesThatMatchUrlSegment = _pageBundles.Where(x => x.Page.Url == lastUrlSegment);
                foreach (PageBundle pageBundle in _pageBundlesThatMatchUrlSegment)
                {
                    int count = 0;
                    int parent = pageBundle.Page.Parent;
                    bool nextForEachPage = false;

                    //If there is only one item segment you just need to skip the foreach, otherwise there is a change the code will take the wrong route
                    if (urlSplitList.Count() == 1)
                    {
                        //Check is this item doesn't have a parent
                        if (pageBundle.Page.Parent != 0) { continue; }
                    }
                    else
                    {
                        foreach (string urlSegment in Enumerable.Reverse(urlSplitList))
                        {
                            //Skip the first item, because this is the last url segment and you don't need to check this one
                            if (count++ == 0) { continue; }

                            //Get parent page by urlSegment and parent
                            Pages _page = _pageBundles.Where(x => x.Page.Url == urlSegment)
                                                      .Where(x => x.Page.Id == parent)
                                                      .Select(x => x.Page)
                                                      .FirstOrDefault();

                            //If there isn't find a parent page then we need to chech another page that match the last url segment
                            if (_page == null)
                            {
                                nextForEachPage = true;
                                break;
                            }
                            else
                            {
                                parent = _page.Parent;
                            }
                        }

                        //Check if the segment for each has failed
                        if (nextForEachPage) { continue; }
                    }

                    Routing.ParentUrlParents parentUrlParents = new Routing.ParentUrlParents();

                    if (pageBundle.Page.Parent != 0)
                    {
                        Routing.FindParentUrlInPageBundlesAndCreateList(pageBundle.Page.Parent, pageBundle.Page.Url, _pageBundles, parentUrlParents).Remove(0, 1); //Remove first character because that is a / and that will give a error
                    }

                    RouteValueDictionary routeValueDictionary = new RouteValueDictionary()
                    {
                        { "type", "spine-fallback" },
                        { "id", (int)pageBundle.Page.Id },
                        { "websiteLanguageId", _languageBundle.WebsiteLanguage.Id },
                        { "defaultLanguage", _languageBundle.WebsiteLanguage.DefaultLanguage },
                        { "code", _languageBundle.Language.Code },
                        { "culture", _languageBundle.Language.Culture },
                        { "timeZoneId", _languageBundle.Language.TimeZoneId },
                        { "parentUrlParents", JsonConvert.SerializeObject(parentUrlParents.List) }
                    };

                    string output = "";
                    string baseUrl = Request.Scheme + "://" + Request.Host;
                    string action = pageBundle.PageTemplate.Action;
                    string controller = pageBundle.PageTemplate.Controller;
                    string urlAction = Url.Action(action, controller, routeValueDictionary);
                    WebRequest webRequest = WebRequest.Create(string.Format("{0}{1}", baseUrl, urlAction));
                    try
                    {
                        WebResponse webResponse = await webRequest.GetResponseAsync();
                        if (webResponse.GetResponseStream().CanRead)
                        {
                            StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                            output = reader.ReadToEnd();
                        }
                    }
                    catch
                    {
                        return RedirectToRoute("404");
                    }

                    return Content(output, "text/html");
                }
            }

            return RedirectToRoute("404");
        }
    }
}