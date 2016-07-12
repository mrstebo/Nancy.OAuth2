using System.Collections.Generic;
using Nancy.Bootstrapper;

namespace Nancy.OAuth2.Tests
{
    internal static class BootstrapperExtensions
    {
        public static void WithSession(this IPipelines pipeline, IDictionary<string, object> session)
        {
            pipeline.BeforeRequest.AddItemToEndOfPipeline(ctx =>
            {
                ctx.Request.Session = new Session.Session(session);
                return null;
            });
        }
    }
}
