using Umbraco.Community.Automate.Skoda.Client.Generated;

namespace Umbraco.Community.Automate.Skoda.Client;

internal sealed class SkodaClientFactory(
    IHttpClientFactory httpClientFactory)
    : ISkodaClientFactory
{
    public SkodaApiClient Create(string apiKey)
    {
        var httpClient = httpClientFactory.CreateClient("Skoda");

        httpClient.DefaultRequestHeaders.Remove("X-API-Key");
        httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

        return new SkodaApiClient(httpClient);
    }
}