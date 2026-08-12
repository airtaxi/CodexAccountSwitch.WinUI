using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using CliAccountSwitcher.Api.Providers.OpenCodeGo.Models;
using CliAccountSwitcher.Api.Providers.OpenCodeGo.Models.Usage;
using CliAccountSwitcher.Api.Providers.Serialization;

namespace CliAccountSwitcher.Api.Providers.OpenCodeGo;

public sealed class OpenCodeGoUsageClient(HttpClient httpClient)
{
    public async Task<OpenCodeGoUsageSnapshot> GetUsageAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentException("The API key is required.", nameof(apiKey));

        var requestUri = new Uri(OpenCodeGoApiConventions.ConsoleBaseUri, OpenCodeGoApiConventions.UsageApiPath);
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        if (httpResponseMessage.StatusCode is HttpStatusCode.Unauthorized) throw new OpenCodeGoAuthExpiredException("The OpenCode Go API key is invalid or expired.");
        if (httpResponseMessage.StatusCode is HttpStatusCode.Forbidden) throw new OpenCodeGoAuthExpiredException("The OpenCode Go subscription is required.");

        httpResponseMessage.EnsureSuccessStatusCode();
        var responseText = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        return ParseUsageResponse(responseText);
    }

    private static OpenCodeGoUsageSnapshot ParseUsageResponse(string responseText)
    {
        var snapshot = new OpenCodeGoUsageSnapshot { RawResponseText = responseText };

        OpenCodeGoUsageApiResponse response;
        try
        {
            response = JsonSerializer.Deserialize(responseText, ProviderJsonSerializerContext.Default.OpenCodeGoUsageApiResponse) ?? new OpenCodeGoUsageApiResponse();
        }
        catch
        {
            return snapshot;
        }

        snapshot.RollingUsage = ParseUsageWindow(response.Usage.Rolling);
        snapshot.WeeklyUsage = ParseUsageWindow(response.Usage.Weekly);
        snapshot.MonthlyUsage = ParseUsageWindow(response.Usage.Monthly);

        return snapshot;
    }

    private static OpenCodeGoUsageWindow ParseUsageWindow(OpenCodeGoUsageApiWindow usageWindow)
    {
        if (usageWindow is null) return new OpenCodeGoUsageWindow();
        if (!string.Equals(usageWindow.Status, "ok", StringComparison.Ordinal) && !string.Equals(usageWindow.Status, "rate-limited", StringComparison.Ordinal)) return new OpenCodeGoUsageWindow();
        if (usageWindow.ResetsAt is not DateTimeOffset resetAt) return new OpenCodeGoUsageWindow();

        var resetAfterSeconds = Math.Max(0, (long)(resetAt - DateTimeOffset.UtcNow).TotalSeconds);

        return new OpenCodeGoUsageWindow
        {
            UsedPercentage = usageWindow.Percent,
            RemainingPercentage = 100 - usageWindow.Percent,
            ResetAfterSeconds = resetAfterSeconds,
            ResetAt = resetAt
        };
    }
}
