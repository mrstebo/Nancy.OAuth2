using Nancy.Bootstrapper;
using Nancy.OAuth2.Services;
using Nancy.TinyIoc;

namespace Nancy.OAuth2.IntegrationTests
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            OAuth.Enable();

            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            container.Register(StubFactory.Get<ITokenEndpointService>());

            base.ConfigureRequestContainer(container, context);
        }
    }
}
