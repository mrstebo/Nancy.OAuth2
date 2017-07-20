using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Nancy.OAuth2
{
    public class OAuthConfiguration
    {
        public OAuthConfiguration()
        {
            AuthorizationRequestRoute = "/authorize";
            AuthorizationAllowRoute = "/allow";
            AuthorizationDenyRoute = "/deny";
            Base = "/oauth";
            TokenRoute = "/token";
        }

        public string Base { get; set; }

        public string AuthorizationRequestRoute { get; set; }

        public string AuthorizationAllowRoute { get; set; }

        public string AuthorizationDenyRoute { get; set; }

        public string TokenRoute { get; set; }

        public string GetFullPath(Expression<Func<OAuthConfiguration, object>> expression)
        {
            var member = expression.GetTargetMemberInfo() as PropertyInfo;

            if (member == null)
                throw new InvalidOperationException();

            var value = member.GetValue(this, null);

            return string.Concat(Base, "/", value);
        }
    }
}