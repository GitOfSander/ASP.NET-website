using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class Websites
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Domain { get; set; }
        public string Extension { get; set; }
        public string Folder { get; set; }
        public string Subdomain { get; set; }
        public string TypeClient { get; set; }
        public string RootPageAlternateGuid { get; set; }
        public string Subtitle { get; set; }
        public bool Active { get; set; }
    }
}
