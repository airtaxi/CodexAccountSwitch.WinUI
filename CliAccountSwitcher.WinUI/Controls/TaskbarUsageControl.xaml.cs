using CliAccountSwitcher.WinUI.Helpers;
using CliAccountSwitcher.WinUI.Views;
using CliAccountSwitcher.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CliAccountSwitcher.WinUI.Controls;

public sealed partial class TaskbarUsageControl : UserControl, IDisposable
{
    private const double RefreshButtonVisibilityWidthOffset = 26;
    private const double CompactHeightThreshold = 40;

    private static readonly Thickness s_rootButtonDefaultMargin = new(4);
    private static readonly Thickness s_rootButtonDefaultPadding = new(4);
    private static readonly Thickness s_rootButtonCompactMargin = new(4, 0, 4, 0);
    private static readonly Thickness s_rootButtonCompactPadding = new(4, 0, 4, 0);

    private bool _disposed;

    public TaskbarUsageControlViewModel ViewModel { get; }

    public TaskbarUsageControl()
    {
        ViewModel = App.Services.GetRequiredService<TaskbarUsageControlViewModel>();

        InitializeComponent();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        ViewModel.Dispose();
        GC.SuppressFinalize(this);
    }

    private async void OnTaskbarUsageControlLoaded(object sender, RoutedEventArgs e) => await ViewModel.ReloadUsageOrRefreshMissingActiveUsageAsync();

    private void OnRootButtonClicked(object sender, RoutedEventArgs e) => MainWindow.ShowActiveAccountQuotaPopup();

    private void OnRootButtonSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not Button rootButton) return;

        var isCompact = rootButton.ActualHeight < CompactHeightThreshold;
        rootButton.Margin = isCompact ? s_rootButtonCompactMargin : s_rootButtonDefaultMargin;
        rootButton.Padding = isCompact ? s_rootButtonCompactPadding : s_rootButtonDefaultPadding;
    }

    private void OnButtonGridSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not FrameworkElement rootElement) return;

        var shouldShowRefreshButton = rootElement.ActualWidth > TaskbarHelper.PreferredTaskbarContentWidth - RefreshButtonVisibilityWidthOffset;
        RefreshActiveAccountButton.Visibility = shouldShowRefreshButton ? Visibility.Visible : Visibility.Collapsed;
    }
}
