using CliAccountSwitcher.Api.Providers.Abstractions;

namespace CliAccountSwitcher.WinUI.Helpers;

public static class UsagePacemakerHelper
{
    public static bool TryGetUsagePercentages(ProviderUsageWindow providerUsageWindow, TimeSpan usageWindowDuration, out int remainingPercentage, out int pacemakerPercentage, out int pacemakerDifferencePercentage)
    {
        remainingPercentage = 0;
        pacemakerPercentage = 0;
        pacemakerDifferencePercentage = 0;

        if (providerUsageWindow is null || providerUsageWindow.RemainingPercentage < 0 || providerUsageWindow.ResetAfterSeconds < 0) return false;
        if (usageWindowDuration <= TimeSpan.Zero) return false;

        remainingPercentage = Math.Clamp(providerUsageWindow.RemainingPercentage, 0, 100);
        var pacemakerRemainingPercentage = Math.Clamp((TimeSpan.FromSeconds(providerUsageWindow.ResetAfterSeconds).TotalSeconds / usageWindowDuration.TotalSeconds) * 100.0, 0.0, 100.0);
        pacemakerPercentage = Convert.ToInt32(Math.Round(pacemakerRemainingPercentage, MidpointRounding.AwayFromZero));
        pacemakerDifferencePercentage = CalculateUsagePacemakerDifferencePercentage(providerUsageWindow, remainingPercentage, usageWindowDuration);
        return true;
    }

    public static int GetRemainingProgressBarZIndex(int remainingPercentage, int pacemakerPercentage, int pacemakerDifferencePercentage) => pacemakerDifferencePercentage < 0 || pacemakerDifferencePercentage == 0 && remainingPercentage <= pacemakerPercentage ? 1 : 0;

    public static int GetPacemakerProgressBarZIndex(int remainingPercentage, int pacemakerPercentage, int pacemakerDifferencePercentage) => pacemakerDifferencePercentage > 0 || pacemakerDifferencePercentage == 0 && pacemakerPercentage < remainingPercentage ? 1 : 0;

    private static int CalculateUsagePacemakerDifferencePercentage(ProviderUsageWindow providerUsageWindow, int remainingPercentage, TimeSpan usageWindowDuration)
    {
        var usedPercentage = providerUsageWindow.UsedPercentage is >= 0 and <= 100 ? providerUsageWindow.UsedPercentage : 100 - remainingPercentage;
        var elapsedDuration = usageWindowDuration - TimeSpan.FromSeconds(providerUsageWindow.ResetAfterSeconds);
        if (elapsedDuration < TimeSpan.Zero) elapsedDuration = TimeSpan.Zero;

        var averagePaceUsedPercentage = Math.Clamp((elapsedDuration.TotalSeconds / usageWindowDuration.TotalSeconds) * 100.0, 0.0, 100.0);
        var usageAverageRateDifferencePercentage = usedPercentage - averagePaceUsedPercentage;
        if (usageAverageRateDifferencePercentage > 0) return -Convert.ToInt32(Math.Ceiling(usageAverageRateDifferencePercentage));
        if (usageAverageRateDifferencePercentage < 0) return Convert.ToInt32(Math.Ceiling(-usageAverageRateDifferencePercentage));
        return 0;
    }
}
