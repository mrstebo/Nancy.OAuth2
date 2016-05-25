using System;
using Nancy.ModelBinding;
using Nancy.OAuth2.Models;

namespace Nancy.OAuth2.ModelBinders
{
    public class TokenRequestBinder : IModelBinder
    {
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration,
            params string[] blackList)
        {
            return new TokenRequest
            {
                GrantType = context.Request.Form["grant_type"],
                Username = context.Request.Form["username"],
                Password = context.Request.Form["password"],
                Code = context.Request.Form["code"],
                RedirectUri = context.Request.Form["redirect_uri"],
                Scope = context.Request.Form["scope"]
            };
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof (TokenRequest);
        }
    }
}
