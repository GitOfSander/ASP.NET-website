using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using System.Collections.Generic;
using System.Linq;
using static Site.Startup;

namespace Site.Models
{
    public class Website : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Website(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class WebsiteBundle
        {
            public Languages Language { get; set; }
            public IEnumerable<Languages> Languages { get; set; }
            public IEnumerable<WebsiteFields> WebsiteFields { get; set; }
            public WebsiteFields WebsiteField { get; set; }
            public IEnumerable<WebsiteFiles> WebsiteFiles { get; set; }
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public IEnumerable<WebsiteLanguages> WebsiteLanguages { get; set; }
            public IEnumerable<WebsiteResources> WebsiteResources { get; set; }
            public WebsiteResources WebsiteResource { get; set; }
            public Websites Website { get; set; }
            public IEnumerable<WebsiteUploads> WebsiteUploads { get; set; }
        }

        public string GetWebsiteUrl(int WebsiteLanguageId)
        {
            Websites Website = _context.WebsiteLanguages.Join(_context.Websites, WebsiteLanguages => WebsiteLanguages.WebsiteId, Websites => Websites.Id, (WebsiteLanguages, Websites) => new { WebsiteLanguages, Websites })
                                                        .Where(x => x.WebsiteLanguages.Id == WebsiteLanguageId)
                                                        .Select(x => x.Websites)
                                                        .FirstOrDefault();

            string Subdomain = "";
            if (Website.Subdomain != "")
            {
                Subdomain = Website.Subdomain + ".";
            }

            return Website.TypeClient + "://" + Subdomain + Website.Domain + "." + Website.Extension;
        }

        public WebsiteBundle GetWebsiteBundle(int WebsiteLanguageId)
        {
            WebsiteBundle WebsiteBundle = null;

            WebsiteBundle = _context.Websites.Join(_context.WebsiteLanguages, Websites => Websites.Id, WebsiteLanguages => WebsiteLanguages.WebsiteId, (Websites, WebsiteLanguages) => new { Websites, WebsiteLanguages })
                                             .GroupJoin(_context.WebsiteFiles.Join(_context.WebsiteUploads, WebsiteFiles => WebsiteFiles.WebsiteUploadId, WebsiteUploads => WebsiteUploads.Id, (WebsiteFiles, WebsiteUploads) => new { WebsiteFiles, WebsiteUploads }), x => x.WebsiteLanguages.Id, x => x.WebsiteFiles.WebsiteLanguageId, (x, WebsiteFiles) => new { x.WebsiteLanguages, x.Websites, WebsiteFiles })
                                             .GroupJoin(_context.WebsiteResources.Join(_context.WebsiteFields, WebsiteResources => WebsiteResources.WebsiteFieldId, WebsiteFields => WebsiteFields.Id, (WebsiteResources, WebsiteFields) => new { WebsiteResources, WebsiteFields }), x => x.WebsiteLanguages.Id, x => x.WebsiteResources.WebsiteLanguageId, (x, WebsiteResources) => new { x.WebsiteLanguages, x.Websites, x.WebsiteFiles, WebsiteResources })
                                             .Where(x => x.WebsiteLanguages.Active == true)
                                             .Where(x => x.WebsiteLanguages.Id == WebsiteLanguageId)
                                             .Select(x => new WebsiteBundle()
                                             {
                                                 Languages = null,
                                                 WebsiteFields = x.WebsiteResources.Select(y => y.WebsiteFields),
                                                 WebsiteFiles = x.WebsiteFiles.Select(y => y.WebsiteFiles),
                                                 WebsiteLanguage = x.WebsiteLanguages,
                                                 WebsiteLanguages = null,
                                                 WebsiteResources = x.WebsiteResources.Select(y => y.WebsiteResources),
                                                 Website = x.Websites,
                                                 WebsiteUploads = x.WebsiteFiles.Select(y => y.WebsiteUploads)
                                             })
                                             .FirstOrDefault();

            return WebsiteBundle;
        }

        public WebsiteBundle ChangeTextByType(WebsiteBundle websiteBundle)
        {
            foreach (WebsiteResources websiteResource in websiteBundle.WebsiteResources)
            {
                string type = websiteBundle.WebsiteFields.FirstOrDefault(WebsiteFields => WebsiteFields.Id == websiteResource.WebsiteFieldId).Type;
                string text = websiteResource.Text;
                if (type.ToLower() == "textarea")
                {
                    websiteBundle.WebsiteResources.FirstOrDefault(WebsiteResources => WebsiteResources.Id == websiteResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                }
            }

            return websiteBundle;
        }

        [Route("/spine-api/website")]
        [HttpGet]
        public IActionResult GetWebsiteBundleApi(int websiteLanguageId)
        {
            try
            {
                string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

                WebsiteBundle _websiteBundle = GetWebsiteBundle(websiteLanguageId);

                List<Dictionary<string, object>> websiteFilesParentRow = new List<Dictionary<string, object>>();
                Dictionary<string, object> websiteFilesChildRow;
                foreach (WebsiteFiles websiteFile in _websiteBundle.WebsiteFiles)
                {
                    if (websiteFile.Active)
                    {
                        websiteFilesChildRow = new Dictionary<string, object>()
                        {
                            { "callName", _websiteBundle.WebsiteUploads.FirstOrDefault(WebsiteUploads => WebsiteUploads.Id == websiteFile.WebsiteUploadId).CallName},
                            { "originalPath", url + websiteFile.OriginalPath.Replace("~/", "/")},
                            { "compressedPath", url + websiteFile.CompressedPath.Replace("~/", "/")},
                            { "alt", websiteFile.Alt},
                            { "customOrder", websiteFile.CustomOrder}
                        };
                        websiteFilesParentRow.Add(websiteFilesChildRow);
                    }
                }

                List<Dictionary<string, object>> websiteResourcesParentRow = new List<Dictionary<string, object>>();
                Dictionary<string, object> websiteResourcesChildRow;
                foreach (WebsiteResources websiteResource in _websiteBundle.WebsiteResources)
                {
                    string type = _websiteBundle.WebsiteFields.FirstOrDefault(WebsiteFields => WebsiteFields.Id == websiteResource.WebsiteFieldId).Type;
                    string text = websiteResource.Text;
                    if (type.ToLower() == "textarea")
                    {
                        text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }
                    websiteResourcesChildRow = new Dictionary<string, object>()
                    {
                        { "callName", _websiteBundle.WebsiteFields.FirstOrDefault(WebsiteFields => WebsiteFields.Id == websiteResource.WebsiteFieldId).CallName},
                        { "text", text}
                    };
                    websiteResourcesParentRow.Add(websiteResourcesChildRow);
                }

                return Ok(Json(new
                {
                    website = _websiteBundle.Website,
                    websiteFiles = websiteFilesParentRow,
                    websiteResources = websiteResourcesParentRow
                }));
            }
            catch
            {
                return StatusCode(400, Json(new
                {
                    website = "",
                    websiteFiles = "",
                    websiteResources = ""
                }));
            }
        }
    }
}