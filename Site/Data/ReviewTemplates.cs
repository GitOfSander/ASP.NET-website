namespace Site.Data
{
    public partial class ReviewTemplates
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string CallName { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool CheckBeforeOnline { get; set; }
        public string LinkedToType { get; set; }
    }
}
