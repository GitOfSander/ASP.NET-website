namespace Site.Data
{
    public partial class Pages
    {
        public int Id { get; set; }
        public int WebsiteLanguageId { get; set; }
        public int PageTemplateId { get; set; }
        public int Parent { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string AlternateGuid { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
    }
}
