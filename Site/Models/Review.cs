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
using static Site.Startup;
using Microsoft.Extensions.Options;
using Site.Models.App;

namespace Site.Models
{
    public class Review : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Review(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class ReviewBundle
        {
            public Reviews Review { get; set; }
            public ReviewTemplates ReviewTemplate { get; set; }
        }

        public bool InsertReview(string CallName, int WebsiteLanguageId, int LinkedToId, string UserId, string Name, string Email, string Text, byte Rating)
        {
            ReviewTemplates _reviewTemplate = _context.ReviewTemplates.Where(x => x.WebsiteId == 1).FirstOrDefault(x => x.CallName == CallName);

            DateTime UtcTime = DateTime.UtcNow;
            TimeZoneInfo Tzi = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime CreatedAt = TimeZoneInfo.ConvertTime(UtcTime, Tzi); // convert from utc to local

            bool Active = true;
            if (_reviewTemplate.CheckBeforeOnline == true)
            {
                Active = false;
            }

            var _review = new Reviews { WebsiteLanguageId = WebsiteLanguageId, LinkedToId = LinkedToId, UserId = UserId, Name = Name, Email = Email, Text = Text, Rating = Rating, Active = Active, CreatedAt = CreatedAt, ViewedByAdmin = false, ReviewTemplateId = _reviewTemplate.Id };
            _context.Reviews.Add(_review);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<ReviewBundle> GetReviewsByWebsiteIdAndCallName(int WebsiteId, string CallName)
        {
            IEnumerable<ReviewBundle> ReviewBundle = null;

            ReviewBundle = _context.Reviews.Join(_context.ReviewTemplates, Reviews => Reviews.ReviewTemplateId, ReviewTemplates => ReviewTemplates.Id, (Reviews, ReviewTemplates) => new { Reviews, ReviewTemplates })
                                       .Where(x => x.Reviews.Active == true)
                                       .Where(x => x.ReviewTemplates.WebsiteId == WebsiteId)
                                       .Where(x => x.ReviewTemplates.CallName == CallName)
                                       .Select(x => new ReviewBundle() {
                                           Review = x.Reviews,
                                           ReviewTemplate = x.ReviewTemplates
                                       }).ToList();

            return ReviewBundle;
        }
    }
}
