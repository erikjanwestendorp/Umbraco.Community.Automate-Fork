# Umbraco Community Automate Cloudflare

A Cloudflare integration for [Umbraco Automate](https://github.com/umbraco/Umbraco.Automate).

The package adds Cloudflare connectivity and actions to Automate, allowing Cloudflare cache operations to become part of configurable automation workflows.

## Getting Started

```bash
dotnet add package Umbraco.Community.Automate.Cloudflare
```

## Cloudflare Connection

The package adds a **Cloudflare connection type** to Umbraco Automate.

Create a Cloudflare connection in the Automate backoffice and configure:

| Setting | Description |
| --- | --- |
| Account ID | Your Cloudflare Account ID. |
| Zone ID | The Cloudflare Zone ID containing the website. |
| API Token | A Cloudflare API Token with the required permissions. |

The connection validates that the configured API token is valid and active.

It is recommended to use an API token with only the minimum permissions required by the actions used in your workflows.

### Keeping values in configuration

Instead of typing values into the connection, you can keep them in `appsettings.json` (or user secrets / environment variables) under `Umbraco:Community:Automate:Cloudflare` and reference them from the connection fields:

```json
{
  "Umbraco": {
    "Community": {
      "Automate": {
        "Cloudflare": {
          "Variables": {
            "AccountId": "<account-id>",
            "ZoneId": "<zone-id>"
          },
          "Secrets": {
            "ApiToken": "<api-token>"
          }
        }
      }
    }
  }
}
```

| Field | Value |
| --- | --- |
| Account ID | `$Umbraco:Community:Automate:Cloudflare:Variables:AccountId` |
| Zone ID | `$Umbraco:Community:Automate:Cloudflare:Variables:ZoneId` |
| API Token | `$Umbraco:Community:Automate:Cloudflare:Secrets:ApiToken` |

Values under `Secrets` are treated as secrets by Automate.

## Purge Cloudflare URLs

The **Purge Cloudflare URLs** action removes one or more absolute URLs from the Cloudflare cache.

This allows cache invalidation to become part of an Automate workflow instead of requiring custom event handlers or application-specific Cloudflare integrations.

The **URLs** setting is a binding expression that resolves to the URLs to purge, for example `${previous.urls}`. It accepts a collection of URLs, a JSON array, or a single URL. URLs are trimmed and de-duplicated, and all of them are purged in a single Cloudflare API request.

For example:

```text
Content Published
        ↓
Get Published Content URLs
        ↓
Purge Cloudflare URLs
```

### Output

| Property | Description |
| --- | --- |
| `PurgedUrlCount` | The number of URLs purged. |
| `PurgedUrls` | The URLs that were purged. |

## Using Published Content URLs

For content-based workflows, this package can be combined with an action that resolves all URLs for a published content item and culture, such as **Get Published Content URLs** from `Umbraco.Community.Automate.Extensions`.

This is particularly useful for multilingual and multi-domain Umbraco installations.

A workflow can therefore:

1. React to content being published.
2. Resolve the published URLs for the affected culture.
3. Purge those URLs from Cloudflare.

The individual actions remain independent, allowing them to be combined with other Automate triggers and actions as needed.

## Cloudflare API

Cache purging uses the Cloudflare API and requires a valid API token with permission to purge cache for the configured zone.

## Documentation

For documentation, examples and source code, visit:

https://github.com/umbraco-community/Umbraco.Community.Automate
