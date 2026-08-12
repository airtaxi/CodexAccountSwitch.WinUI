using CliAccountSwitcher.Api.Providers.OpenCodeGo.Models;

namespace CliAccountSwitcher.Api.Providers.OpenCodeGo.Infrastructure;

public static class OpenCodeGoAuthFileReader
{
    public static async Task<OpenCodeGoAuthFileInfo> LoadAsync(string authFilePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(authFilePath)) throw new FileNotFoundException("The OpenCode Go auth file was not found.", authFilePath);
        var authFileText = await File.ReadAllTextAsync(authFilePath, cancellationToken);
        return OpenCodeGoAuthFileInfo.Parse(authFileText);
    }
}
