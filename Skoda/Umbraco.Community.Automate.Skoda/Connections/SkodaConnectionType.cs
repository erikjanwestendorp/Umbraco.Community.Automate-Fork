using Umbraco.Automate.Core.Connections;
using Umbraco.Community.Automate.Skoda.Client;

namespace Umbraco.Community.Automate.Skoda.Connections;

[ConnectionType(
    SkodaConstants.ConnectionTypeAlias,
    "Škoda",
    Description = "Connects Umbraco Automate to a Škoda vehicle using the MyŠkoda Public API.",
    Icon = "icon-car")]
public sealed class SkodaConnectionType(ConnectionTypeInfrastructure infrastructure) : ConnectionTypeBase<SkodaConnectionSettings>(infrastructure)
{
    public override async Task<ConnectionValidationResult> ValidateAsync(
    object? settings,
    CancellationToken cancellationToken)
    {
        if (settings is not SkodaConnectionSettings skodaSettings)
        {
            return ConnectionValidationResult.Failure("Invalid settings.");
        }

        if (string.IsNullOrWhiteSpace(skodaSettings.ApiKey) ||
            string.IsNullOrWhiteSpace(skodaSettings.Vin))
        {
            return ConnectionValidationResult.Failure("API key and VIN must be provided.");
        }

        //skodaClient.GetVehicleStatusAsync()

        //using var response = await skodaClient.GetVehicleStatusAsync(
        //    settings.ApiKey,
        //    settings.Vin,
        //    cancellationToken);

        return ConnectionValidationResult.Success();
        //return response.IsSuccessStatusCode;
    }
}
