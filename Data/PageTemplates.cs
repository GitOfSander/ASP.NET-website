using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class PageTemplates
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
