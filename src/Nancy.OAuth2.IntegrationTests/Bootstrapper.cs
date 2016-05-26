using System;
using Nancy.Bootstrapper;
using Nancy.OAuth2.Services;
using Nancy.Session;
using Nancy.TinyIoc;

namespace Nancy.OAuth2.IntegrationTests
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        public Action<NancyContext> RequestInjection { get; set; }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            OAuth.Enable();

            base.ApplicationStartup(container, pipelines);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            if (RequestInjection != null)
                RequestInjection(context);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            container.Register(StubFactory.Get<ITokenEndpointService>());

            base.ConfigureRequestContainer(container, context);
        }
    }
}
