using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Automate.Skoda.Client;

namespace Umbraco.Community.Automate.Skoda.Compose;

internal static class UmbracoBuilderExtensions
{
    extension(IUmbracoBuilder builder)
    {
        public IUmbracoBuilder AddSkodaAutomate()
        {
            builder.Services.AddHttpClient<ISkodaClient, SkodaClient>(client =>
            {
                client.BaseAddress = new Uri("https://public.api.connect.skoda-auto.cz/");
            });

            return builder;
        }
    }
}
