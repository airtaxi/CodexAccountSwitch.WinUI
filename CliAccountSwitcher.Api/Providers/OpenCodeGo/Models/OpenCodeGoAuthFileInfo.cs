using System.Text.Json;

namespace CliAccountSwitcher.Api.Providers.OpenCodeGo.Models;

public sealed class OpenCodeGoAuthFileInfo
{
    public string ApiKey { get; set; } = "";

    public bool IsValid => !string.IsNullOrWhiteSpace(ApiKey);

    public static OpenCodeGoAuthFileInfo Parse(string authFileText)
    {
        var authFileInfo = new OpenCodeGoAuthFileInfo();
        try
        {
            using var jsonDocument = JsonDocument.Parse(authFileText);
            if (!jsonDocument.RootElement.TryGetProperty(OpenCodeGoApiConventions.ProviderId, out var openCodeGoElement)) return authFileInfo;
            if (!openCodeGoElement.TryGetProperty("type", out var typeElement) || !string.Equals(typeElement.GetString(), "api", StringComparison.Ordinal)) return authFileInfo;
            if (!openCodeGoElement.TryGetProperty("key", out var keyElement)) return authFileInfo;
            authFileInfo.ApiKey = keyElement.GetString() ?? "";
        }
        catch { }

        return authFileInfo;
    }
}
