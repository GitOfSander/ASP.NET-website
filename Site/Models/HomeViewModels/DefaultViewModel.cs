using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Site.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Site.Startup;
using static Site.Models.Data;
using static Site.Models.Navigation;
using static Site.Models.Page;
using static Site.Models.Review;
using static Site.Models.Routing;
using static Site.Models.Website;

namespace Site.Models.HomeViewModels
{
    public class DefaultViewModel
    {
        public IDictionary<string, IEnumerable<DataBundle>> DataBundles { get; set; }
        public IDictionary<string, DataBundle> DataBundle { get; set; }
        public ParentBreadcrumbs Breadcrumbs { get; set; }
        public IDictionary<string, string> Links { get; set; }
        public IDictionary<string, List<NavigationLinks>> NavigationLinks { get; set; }
        public PageBundle PageBundle { get; set; }
        public IDictionary<string, IEnumerable<ReviewBundle>> ReviewBundles { get; set; }
        public WebsiteBundle WebsiteBundle { get; set; }

        public string GetPageResourceByCallName(PageBundle PageBundle, string CallName)
        {
            PageBundle _pageBundle = PageBundle.PageTemplateFields.Join(PageBundle.PageResources, PageTemplateFields => PageTemplateFields.Id, PageResources => PageResources.PageTemplateFieldId, (PageTemplateFields, PageResources) => new { PageTemplateFields, PageResources })
                                                                  .Where(x => x.PageTemplateFields.CallName == CallName)
                                                                  .Select(x => new PageBundle()
                                                                  {
                                                                      PageTemplateField = x.PageTemplateFields,
                                                                      PageResource = x.PageResources
                                                                  }).FirstOrDefault();

            string Text = "";
            if (_pageBundle != null)
            {
                Text = _pageBundle.PageResource.Text;
            }

            return Text;
        }

        public string GetPageFileCompressedByCallName(PageBundle PageBundle, string CallName)
        {
            PageBundle _pageBundle = PageBundle.PageTemplateUploads.Join(PageBundle.PageFiles, PageTemplateUploads => PageTemplateUploads.Id, PageFiles => PageFiles.PageTemplateUploadId, (PageTemplateUploads, PageFiles) => new { PageTemplateUploads, PageFiles })
                                                                   .Where(x => x.PageTemplateUploads.CallName == CallName)
                                                                   .Select(x => new PageBundle()
                                                                   {
                                                                       PageTemplateUpload = x.PageTemplateUploads,
                                                                       PageFile = x.PageFiles
                                                                   }).FirstOrDefault();

            string Text = "";
            if (_pageBundle != null)
            {
                Text = _pageBundle.PageFile.CompressedPath.Replace("~/", "/");
            }

            return Text;
        }

        public string GetPageFileOriginalByCallName(PageBundle PageBundle, string CallName)
        {
            PageBundle _pageBundle = PageBundle.PageTemplateUploads.Join(PageBundle.PageFiles, PageTemplateUploads => PageTemplateUploads.Id, PageFiles => PageFiles.PageTemplateUploadId, (PageTemplateUploads, PageFiles) => new { PageTemplateUploads, PageFiles })
                                                                   .Where(x => x.PageTemplateUploads.CallName == CallName)
                                                                   .Select(x => new PageBundle()
                                                                   {
                                                                       PageTemplateUpload = x.PageTemplateUploads,
                                                                       PageFile = x.PageFiles
                                                                   }).FirstOrDefault();

            string Text = "";
            if (_pageBundle != null)
            {
                Text = _pageBundle.PageFile.OriginalPath.Replace("~/", "/");
            }

            return Text;
        }

        public PageFiles GetPageFileByCallName(PageBundle PageBundle, string CallName)
        {
            PageFiles _pageFile = PageBundle.PageTemplateUploads.Join(PageBundle.PageFiles, PageTemplateUploads => PageTemplateUploads.Id, PageFiles => PageFiles.PageTemplateUploadId, (PageTemplateUploads, PageFiles) => new { PageTemplateUploads, PageFiles })
                                                                .Where(x => x.PageTemplateUploads.CallName == CallName)
                                                                .Select(x => x.PageFiles)
                                                                .FirstOrDefault();

            return _pageFile;
        }

        public IEnumerable<PageFiles> GetPageFilesByCallName(PageBundle PageBundle, string CallName)
        {
            IEnumerable<PageFiles> _pageFiles = PageBundle.PageFiles.Join(PageBundle.PageTemplateUploads.GroupBy(x => x.CallName), PageFiles => PageFiles.PageTemplateUploadId, PageTemplateUploads => PageTemplateUploads.FirstOrDefault().Id, (PageFiles, PageTemplateUploads) => new { PageFiles, PageTemplateUploads })
                                                                    .Where(x => x.PageTemplateUploads.FirstOrDefault().CallName == CallName)
                                                                    .Select(x => x.PageFiles);

            return _pageFiles;
        }

        public string GetDataItemResourceByCallName(DataBundle DataBundle, string CallName)
        {
            DataBundle _dataBundle = DataBundle.DataTemplateFields.Join(DataBundle.DataItemResources, DataTemplateFields => DataTemplateFields.Id, DataItemResources => DataItemResources.DataTemplateFieldId, (DataTemplateFields, DataItemResources) => new { DataTemplateFields, DataItemResources })
                                                                  .Where(x => x.DataTemplateFields.CallName == CallName)
                                                                  .Select(x => new DataBundle()
                                                                  {
                                                                      DataTemplateField = x.DataTemplateFields,
                                                                      DataItemResource = x.DataItemResources
                                                                  }).FirstOrDefault();

            string Text = "";
            if (_dataBundle != null)
            {
                Text = _dataBundle.DataItemResource.Text;
            }

            return Text;
        }

        public string GetDataItemFileCompressedByCallName(DataBundle DataBundle, string CallName)
        {
            DataBundle _dataBundle = DataBundle.DataTemplateUploads.Join(DataBundle.DataItemFiles, DataTemplateUploads => DataTemplateUploads.Id, DataItemFiles => DataItemFiles.DataTemplateUploadId, (DataTemplateUploads, DataItemFiles) => new { DataTemplateUploads, DataItemFiles })
                                                                   .Where(x => x.DataTemplateUploads.CallName == CallName)
                                                                   .Select(x => new DataBundle()
                                                                   {
                                                                       DataTemplateUpload = x.DataTemplateUploads,
                                                                       DataItemFile = x.DataItemFiles
                                                                   }).FirstOrDefault();

            string Text = "";
            if (_dataBundle != null)
            {
                Text = _dataBundle.DataItemFile.CompressedPath.Replace("~/", "/");
            }

            return Text;
        }

        public string GetDataItemFileOriginalByCallName(DataBundle DataBundle, string CallName)
        {
            DataBundle _dataBundle = DataBundle.DataTemplateUploads.Join(DataBundle.DataItemFiles, DataTemplateUploads => DataTemplateUploads.Id, DataItemFiles => DataItemFiles.DataTemplateUploadId, (DataTemplateUploads, DataItemFiles) => new { DataTemplateUploads, DataItemFiles })
                                                                   .Where(x => x.DataTemplateUploads.CallName == CallName)
                                                                   .Select(x => new DataBundle()
                                                                   {
                                                                       DataTemplateUpload = x.DataTemplateUploads,
                                                                       DataItemFile = x.DataItemFiles
                                                                   }).FirstOrDefault();

            string Text = "";
            if (_dataBundle != null)
            {
                Text = _dataBundle.DataItemFile.OriginalPath.Replace("~/", "/");
            }

            return Text;
        }

        public DataItemFiles GetDataItemFileByCallName(DataBundle DataBundle, string CallName)
        {
            DataItemFiles _dataItemFile = DataBundle.DataTemplateUploads.Join(DataBundle.DataItemFiles, DataTemplateUploads => DataTemplateUploads.Id, DataItemFiles => DataItemFiles.DataTemplateUploadId, (DataTemplateUploads, DataItemFiles) => new { DataTemplateUploads, DataItemFiles })
                                                                   .Where(x => x.DataTemplateUploads.CallName == CallName)
                                                                   .Select(x => x.DataItemFiles)
                                                                   .FirstOrDefault();

            return _dataItemFile;
        }

        public IEnumerable<DataItemFiles> GetDataItemFilesByCallName(DataBundle DataBundle, string CallName)
        {
            IEnumerable<DataItemFiles> _dataItemFiles = DataBundle.DataItemFiles.Join(DataBundle.DataTemplateUploads.GroupBy(x => x.CallName), DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.FirstOrDefault().Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads })
                                                                                .Where(x => x.DataTemplateUploads.FirstOrDefault().CallName == CallName)
                                                                                .Select(x => x.DataItemFiles);

            return _dataItemFiles;
        }

        public string GetWebsiteResourceByCallName(WebsiteBundle WebsiteBundle, string CallName)
        {
            WebsiteBundle _websiteBundle = WebsiteBundle.WebsiteFields.Join(WebsiteBundle.WebsiteResources, WebsiteFields => WebsiteFields.Id, WebsiteResources => WebsiteResources.WebsiteFieldId, (WebsiteFields, WebsiteResources) => new { WebsiteFields, WebsiteResources })
                                                                      .Where(x => x.WebsiteFields.CallName == CallName)
                                                                      .Select(x => new WebsiteBundle()
                                                                      {
                                                                          WebsiteField = x.WebsiteFields,
                                                                          WebsiteResource = x.WebsiteResources
                                                                      }).FirstOrDefault();

            string Text = "";
            if (_websiteBundle != null)
            {
                Text = _websiteBundle.WebsiteResource.Text;
            }

            return Text;
        }

        public string GenerateSlug(string value)
        {
            string str = RemoveAccent(value).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        private string RemoveAccent(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}