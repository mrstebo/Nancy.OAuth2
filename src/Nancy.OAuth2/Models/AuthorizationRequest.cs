using System.Collections.Generic;

namespace Nancy.OAuth2.Models
{
    public class AuthorizationRequest
    {
        public string ResponseType { get; set; }
        public string ClientId { get; set; }
        public string RedirectUrl { get; set; }
        public IEnumerable<string> Scope { get; set; }
        public string State { get; set; }
    }
}
