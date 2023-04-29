using System;

namespace Site.Data
{
    public partial class OAuthTokens
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string CallName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}