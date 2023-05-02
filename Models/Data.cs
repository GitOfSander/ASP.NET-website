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
using Microsoft.Extensions.Options;
using static Site.Startup;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections;
using System.Reflection;
using static Site.Models.Routing;
using Site.Models.App;
using System.Text.RegularExpressions;

namespace Site.Models
{
    public class Data : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Data(SiteContext context = null, IOptions<AppSettings> config = null)
        {
            _context = context;
            _config = config;
        }

        public class DataBundle
        {
            public DataItemFiles DataItemFile { get; set; }
            public IEnumerable<DataItemFiles> DataItemFiles { get; set; }
            public DataItemResources DataItemResource { get; set; }
            public IEnumerable<DataItemResources> DataItemResources { get; set; }
            public DataItems DataItem { get; set; }
            public DataTemplateFields DataTemplateField { get; set; }
            public IEnumerable<DataTemplateFields> DataTemplateFields { get; set; }
            public DataTemplates DataTemplate { get; set; }
            public DataTemplateUploads DataTemplateUpload { get; set; }
            public IEnumerable<DataTemplateUploads> DataTemplateUploads { get; set; }
            public DataTemplates LinkedToDataTemplate { get; set; }
            public IEnumerable<DataItems> LinkedToDataItems { get; set; }
        }

        public string GetUrlBeforeDataItemByPageAlternateGuid(int WebsiteLanguageId, string PageAlternateGuid)
        {
            DataRoutes _dataRoute = _context.Pages.Join(_context.WebsiteLanguages, Pages => Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (Pages, WebsiteLanguages) => new { Pages, WebsiteLanguages })
                                                  .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.Pages, x.WebsiteLanguages, Languages })
                                                  .Where(x => x.WebsiteLanguages.Id == WebsiteLanguageId)
                                                  .Where(x => x.Pages.AlternateGuid == PageAlternateGuid)
                                                  .Where(x => x.WebsiteLanguages.Active == true)
                                                  .Select(x => new DataRoutes()
                                                  {
                                                      DataTemplate = null,
                                                      Page = x.Pages,
                                                      WebsiteLanguage = x.WebsiteLanguages,
                                                      Language = x.Languages
                                                  }).FirstOrDefault();

            List<PageRoutes> _pageRoutes = _context.Pages.Join(_context.PageTemplates, Pages => Pages.PageTemplateId,
                                                              PageTemplates => PageTemplates.Id, (Pages, PageTemplates) => new { Pages, PageTemplates })
                                                         .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.PageTemplates, x.Pages, WebsiteLanguages })
                                                         .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.PageTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                                         .Where(x => x.WebsiteLanguages.Id == WebsiteLanguageId)
                                                         .Select(x => new PageRoutes()
                                                         {
                                                             Page = x.Pages,
                                                             PageTemplate = x.PageTemplates,
                                                             WebsiteLanguage = x.WebsiteLanguages,
                                                             Language = x.Languages
                                                         }).ToList();

            string Url = "";
            if (_dataRoute != null)
            {
                return Url = new Routing().FindParentUrl(_dataRoute.Page.Parent, _dataRoute.Page.Url, _pageRoutes, _dataRoute.WebsiteLanguage.DefaultLanguage, _dataRoute.Language.Code);
            }

            return "/" + Url;
        }

        public DataBundle GetPreviousDataItem(int WebsiteLanguageId, string CallName, int CustomOrder)
        {
            return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                          .Where(x => x.DataTemplates.DetailPage == true)
                                          .Where(x => x.DataTemplates.CallName == CallName)
                                          .Where(x => x.DataItems.WebsiteLanguageId == WebsiteLanguageId)
                                          .Where(x => x.DataItems.Active == true)
                                          .Select(x => new DataBundle()
                                          {
                                              DataItemFiles = null,
                                              DataItemResources = null,
                                              DataItem = x.DataItems,
                                              DataTemplateFields = null,
                                              DataTemplate = x.DataTemplates,
                                              DataTemplateUploads = null
                                          })
                                          .OrderByDescending(x => x.DataItem.CustomOrder)
                                          .FirstOrDefault(x => x.DataItem.CustomOrder < CustomOrder);
        }

        public DataBundle GetNextDataItem(int WebsiteLanguageId, string CallName, int CustomOrder)
        {
            return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .Where(x => x.DataTemplates.DetailPage == true)
                                         .Where(x => x.DataTemplates.CallName == CallName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == WebsiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = null,
                                             DataItemResources = null,
                                             DataItem = x.DataItems,
                                             DataTemplateFields = null,
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = null
                                         })
                                         .OrderBy(x => x.DataItem.CustomOrder)
                                         .FirstOrDefault(x => x.DataItem.CustomOrder > CustomOrder);
        }

        public DataBundle GetDataItem(int WebsiteLanguageId, string CallName, string Url)
        {
            return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                                          .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads }).OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }), x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.DetailPage == true)
                                         .Where(x => x.DataTemplates.CallName == CallName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == WebsiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads)
                                         })
                                         .FirstOrDefault(x => x.DataItem.PageUrl == Url);
        }

        public DataBundle GetDataItemWithCategorie(int WebsiteLanguageId, string CallName, string Url)
        {
            return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                                          .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads })
                                                                          .OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }) //Here comes the DataTemplateFields.LinkedToDataTemplateId
                                                                              .GroupJoin(_context.DataItems.Join(_context.DataTemplates, DataItems => DataItems.DataTemplateId, DataTemplates => DataTemplates.Id, (DataItems, DataTemplates) => new { DataItems, DataTemplates }), x => x.DataItemResources.Text, y => y.DataItems.Id.ToString(), (x, DataItems) => new { x.DataItemResources, x.DataTemplateFields, DataItems }),
                                         x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.DetailPage == true)
                                         .Where(x => x.DataTemplates.CallName == CallName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == WebsiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads),
                                             LinkedToDataTemplate = null,
                                             LinkedToDataItems = x.DataItemResources.Select(y => y.DataItems.FirstOrDefault().DataItems)
                                         })
                                         .FirstOrDefault(x => x.DataItem.PageUrl == Url);
        }

        public IQueryable<DataBundle> GetDataItemsWithCategorie(int WebsiteLanguageId, string CallName)
        {
            return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                    .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads })
                                                    .OrderBy(x => x.DataItemFiles.CustomOrder),
                                         x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources
                                                    .Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields })
                                                    .GroupJoin(_context.DataItems
                                                               .Join(_context.DataTemplates, DataItems => DataItems.DataTemplateId, DataTemplates => DataTemplates.Id, (DataItems, DataTemplates) => new { DataItems, DataTemplates }),
                                                    x => x.DataItemResources.Text, y => y.DataItems.Id.ToString(), (x, DataItems) => new { x.DataItemResources, x.DataTemplateFields, DataItems }),
                                         x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.DetailPage == true)
                                         .Where(x => x.DataTemplates.CallName == CallName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == WebsiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads),
                                             LinkedToDataTemplate = null,
                                             LinkedToDataItems = x.DataItemResources.Select(y => y.DataItems.FirstOrDefault().DataItems)
                                         });
        }

        public IQueryable<DataBundle> GetDataBundles(int websiteLanguageId, string callName, bool files, bool resources)
        {
            if (files == false && resources == false)
            {
                return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                             .Where(x => x.DataTemplates.CallName == callName)
                                             .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                             .Where(x => x.DataItems.Active == true)
                                             .Select(x => new DataBundle()
                                             {
                                                 DataItemFiles = null,
                                                 DataItemResources = null,
                                                 DataItem = x.DataItems,
                                                 DataTemplateFields = null,
                                                 DataTemplate = x.DataTemplates,
                                                 DataTemplateUploads = null
                                             });
            }
            else if (files == true && resources == false)
            {
                return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                             .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                                              .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads })
                                                                              .OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                             .Where(x => x.DataTemplates.CallName == callName)
                                             .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                             .Where(x => x.DataItems.Active == true)
                                             .Select(x => new DataBundle()
                                             {
                                                 DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                                 DataItemResources = null,
                                                 DataItem = x.DataItems,
                                                 DataTemplateFields = null,
                                                 DataTemplate = x.DataTemplates,
                                                 DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads)
                                             });
            }
            else if (files == false && resources == true)
            {
                return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                             .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }), x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, DataItemResources })
                                             .Where(x => x.DataTemplates.CallName == callName)
                                             .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                             .Where(x => x.DataItems.Active == true)
                                             .Select(x => new DataBundle()
                                             {
                                                 DataItemFiles = null,
                                                 DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                                 DataItem = x.DataItems,
                                                 DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                                 DataTemplate = x.DataTemplates,
                                                 DataTemplateUploads = null
                                             });
            }
            else //Now the 2 has to be true
            {
                return _context.DataTemplates.Join(_context.DataItems, DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                             .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                                              .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads })
                                                                              .OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                             .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }), x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                             .Where(x => x.DataTemplates.CallName == callName)
                                             .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                             .Where(x => x.DataItems.Active == true)
                                             .Select(x => new DataBundle()
                                             {
                                                 DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                                 DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                                 DataItem = x.DataItems,
                                                 DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                                 DataTemplate = x.DataTemplates,
                                                 DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads)
                                             });
            }
        }

        public string GetSectionOrFilter(DataRoutes dataRoute, NavigationItems navigationItem)
        {
            DataTemplateSections dataTemplateSection = dataRoute.DataTemplateSections.FirstOrDefault(DataTemplateSections => DataTemplateSections.Id == navigationItem.LinkedToSectionId);
            switch (dataTemplateSection.Type.ToLower())
            {
                case "section":
                    return dataTemplateSection.Section;
                default: // "datafilter"
                    return Regex.Replace(_context.DataItems.FirstOrDefault(DataItems => DataItems.AlternateGuid == navigationItem.FilterAlternateGuid).AlternateGuid, @"[^A-Za-z0-9_\.~]+", "-");
            }
        }

        public DataBundle ChangeDataBundleTextByType(DataBundle dataBundle)
        {
            if (dataBundle != null) { 
                dataBundle.DataItem.Text = dataBundle.DataItem.Text.Replace("\r\n", "\n").Replace("\n", "<br />");

                if (dataBundle.DataItemResources != null)
                {
                    foreach (DataItemResources dataItemResource in dataBundle.DataItemResources)
                    {
                        string type = dataBundle.DataTemplateFields.FirstOrDefault(DataTemplateFields => DataTemplateFields.Id == dataItemResource.DataTemplateFieldId).Type;
                        string text = dataItemResource.Text;
                        if (type.ToLower() == "textarea")
                        {
                            dataBundle.DataItemResources.FirstOrDefault(DataItemResources => DataItemResources.Id == dataItemResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                        }
                    }
                }
            }

            return dataBundle;
        }

        public IQueryable<DataBundle> ChangeDataBundlesTextByType(IQueryable<DataBundle> dataBundles)
        {
            foreach (DataBundle dataBundle in dataBundles)
            {
                dataBundles.FirstOrDefault(x => x.DataItem.Id == dataBundle.DataItem.Id).DataItem.Text = dataBundle.DataItem.Text.Replace("\r\n", "\n").Replace("\n", "<br />");

                if (dataBundle.DataItemResources != null)
                {
                    foreach (DataItemResources dataItemResource in dataBundle.DataItemResources)
                    {
                        string type = dataBundle.DataTemplateFields.FirstOrDefault(DataTemplateFields => DataTemplateFields.Id == dataItemResource.DataTemplateFieldId).Type;
                        string text = dataItemResource.Text;
                        if (type.ToLower() == "textarea")
                        {
                            dataBundles.FirstOrDefault(x => x.DataItem.Id == dataBundle.DataItem.Id).DataItemResources.FirstOrDefault(DataItemResources => DataItemResources.Id == dataItemResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                        }
                    }
                }
            }

            return dataBundles;
        }

        public IQueryable<DataBundle> ChangeDataBundlesFilePaths(IQueryable<DataBundle> dataBundles)
        {
            string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

            foreach (DataBundle dataBundle in dataBundles)
            {
                foreach (DataItemFiles dataItemFile in dataBundle.DataItemFiles)
                {
                    int dataItemFileId = dataItemFile.Id;
                    int dataItemId = dataBundle.DataItem.Id;
                    string compressedPath = dataItemFile.CompressedPath.Replace("~/", url + "/");
                    string originalPath = dataItemFile.OriginalPath.Replace("~/", url + "/");

                    dataBundles.FirstOrDefault(x => x.DataItem.Id == dataItemId).DataItemFiles.FirstOrDefault(DataItemFiles => DataItemFiles.Id == dataItemFileId).CompressedPath = compressedPath;
                    dataBundles.FirstOrDefault(x => x.DataItem.Id == dataItemId).DataItemFiles.FirstOrDefault(DataItemFiles => DataItemFiles.Id == dataItemFileId).OriginalPath = originalPath;
                }
            }

            return dataBundles;
        }

        [Route("/spine-api/data")]
        [HttpGet]
        public IActionResult GetDataBundlesApi(int websiteLanguageId, string callName, bool files, bool resources)
        {
            try
            {
                return Ok(Json(new
                {
                    dataBundles = ChangeDataBundlesFilePaths(ChangeDataBundlesTextByType(GetDataBundles(websiteLanguageId, callName, files, resources)))
                }));
            }
            catch
            {
                return StatusCode(400, Json(new
                {
                    dataBundles = ""
                }));
            }
        }
    }
}
