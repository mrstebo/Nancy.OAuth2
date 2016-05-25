using System.Collections.Generic;

namespace Nancy.OAuth2.Models
{
    public class TokenRequest
    {
        public string GrantType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
        public string RedirectUri { get; set; }
        public IEnumerable<string> Scope { get; set; }
    }
}
