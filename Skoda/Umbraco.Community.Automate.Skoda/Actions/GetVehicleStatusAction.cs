using Umbraco.Automate.Core.Actions;
using Umbraco.Community.Automate.Skoda.Client;
using Umbraco.Community.Automate.Skoda.Client.Generated;
using Umbraco.Community.Automate.Skoda.Connections;

namespace Umbraco.Community.Automate.Skoda.Actions;

[Action(
    "community.automate.skoda.getVehicleStatus",
    "Get Vehicle Status ",
    Group = "Skoda",
    Icon = "icon-car",
    ConnectionTypeAlias = SkodaConstants.ConnectionTypeAlias)]
public class GetVehicleStatusAction(ActionInfrastructure infrastructure, ISkodaClientFactory skodaClientFactory) : ActionBase<GetVehicleStatusSettings,VehicleResponse>(infrastructure)
{
    public override async Task<ActionResult> ExecuteAsync(
        ActionContext context,
        CancellationToken cancellationToken)
    {
        var connection = context.Connection
            ?? throw new InvalidOperationException(
                "A Škoda connection is required.");

        var settings =
            connection.GetSettings<SkodaConnectionSettings>();

        var client = skodaClientFactory.Create(settings.ApiKey);

        var vehicle = await client.GetVehicleAsync(
            settings.Vin,
            cancellationToken: cancellationToken);

        if(vehicle == null)
        {
            throw new InvalidOperationException("Vehicle not found.");            
        }
        return Success(vehicle);
    }
}