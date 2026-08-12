namespace CliAccountSwitcher.Api.Providers.OpenCodeGo.Models.Usage;

public sealed class OpenCodeGoUsageApiWindow
{
    public string Status { get; set; } = "";

    public int Percent { get; set; }

    public DateTimeOffset? ResetsAt { get; set; }
}
