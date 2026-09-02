using Umbraco.Community.Automate.Skoda.Client.Generated;

namespace Umbraco.Community.Automate.Skoda.Client;

public interface ISkodaClientFactory
{
    SkodaApiClient Create(string apiKey);
}