namespace CliAccountSwitcher.Api.Providers.OpenCodeGo;

public static class OpenCodeGoApiConventions
{
    public static Uri ConsoleBaseUri { get; } = new("https://opencode.ai");

    public static string UsageApiPath => "/zen/go/v1/usage";

    public static string ProviderId => "opencode-go";
}
