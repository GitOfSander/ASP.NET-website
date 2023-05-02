using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class WebsiteLanguages
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public int LanguageId { get; set; }
        public bool DefaultLanguage { get; set; }
        public bool Active { get; set; }
    }
}
