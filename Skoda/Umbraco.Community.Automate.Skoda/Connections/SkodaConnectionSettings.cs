using Umbraco.Automate.Core.Settings;

namespace Umbraco.Community.Automate.Skoda.Connections;

public sealed class SkodaConnectionSettings
{
    [Field(
        Label = "API key",
        Description = "The API key generated in the MyŠkoda app.",
        IsSensitive = true)]
    public string ApiKey { get; set; } = string.Empty;

    [Field(
        Label = "VIN",
        Description = "The Vehicle Identification Number of the Škoda vehicle.")]
    public string Vin { get; set; } = string.Empty;
}