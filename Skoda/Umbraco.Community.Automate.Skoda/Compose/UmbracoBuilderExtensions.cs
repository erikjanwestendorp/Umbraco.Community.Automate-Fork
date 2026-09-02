using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Automate.Skoda.Client;
using Umbraco.Community.Automate.Skoda.Client.Generated;

namespace Umbraco.Community.Automate.Skoda.Compose;

internal static class UmbracoBuilderExtensions
{
    extension(IUmbracoBuilder builder)
    {
        public IUmbracoBuilder AddSkodaAutomate()
        {
            builder.Services.AddHttpClient("Skoda", client =>
            {
                client.BaseAddress = new Uri(SkodaConstants.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddSingleton<ISkodaClientFactory, SkodaClientFactory>();

            return builder;
        }
    }
}
