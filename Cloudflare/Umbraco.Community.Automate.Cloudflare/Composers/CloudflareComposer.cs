using Microsoft.Extensions.DependencyInjection;
using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Configuration;
using Umbraco.Automate.Core.Connections;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Automate.Cloudflare.Actions;
using Umbraco.Community.Automate.Cloudflare.Client;
using Umbraco.Community.Automate.Cloudflare.ConnectionTypes;
using Umbraco.Community.Automate.Cloudflare.Settings;

namespace Umbraco.Community.Automate.Cloudflare.Composers;

public class CloudflareComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddHttpClient<ICloudflareClient, CloudflareClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
        });

        builder.WithCollectionBuilder<ActionCollectionBuilder>()
            .Add<PurgeUrlsAction>();

        builder.WithCollectionBuilder<ConnectionTypeCollectionBuilder>()
            .Add<CloudflareConnectionType>();

        // Automate only resolves $-references under allow-listed prefixes
        builder.Services.PostConfigure<AutomateOptions>(options =>
        {
            options.AllowedConfigurationKeyPrefixes =
            [
                .. options.AllowedConfigurationKeyPrefixes,
                CloudflareConfiguration.VariablesPath,
                CloudflareConfiguration.SecretsPath,
            ];
            options.SecretConfigurationKeyPrefixes =
            [
                .. options.SecretConfigurationKeyPrefixes,
                CloudflareConfiguration.SecretsPath,
            ];
        });
    }
}
