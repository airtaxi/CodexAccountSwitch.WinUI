namespace CliAccountSwitcher.Api.Providers.OpenCodeGo.Models.Usage;

public sealed class OpenCodeGoUsageApiData
{
    public OpenCodeGoUsageApiWindow Rolling { get; set; } = new();

    public OpenCodeGoUsageApiWindow Weekly { get; set; } = new();

    public OpenCodeGoUsageApiWindow Monthly { get; set; } = new();
}
