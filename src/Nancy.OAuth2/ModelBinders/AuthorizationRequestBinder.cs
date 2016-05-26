using System;
using System.Collections.Generic;
using Nancy.ModelBinding;
using Nancy.OAuth2.Models;

namespace Nancy.OAuth2.ModelBinders
{
    public class AuthorizationRequestBinder : IModelBinder
    {
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration,
               params string[] blackList)
        {
            return new AuthorizationRequest
            {
                ResponseType = context.Request.Query["response_type"],
                ClientId = context.Request.Query["client_id"],
                RedirectUrl = context.Request.Query["redirect_url"],
                Scope = SplitAndRemoveEmptyEntries(context.Request.Query["scope"]),
                State = context.Request.Query["state"]
            };
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof(AuthorizationRequest);
        }

        private static IEnumerable<string> SplitAndRemoveEmptyEntries(string s)
        {
            return s.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
