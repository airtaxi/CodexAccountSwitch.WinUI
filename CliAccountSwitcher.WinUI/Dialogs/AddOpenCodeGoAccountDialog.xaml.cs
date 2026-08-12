using CliAccountSwitcher.WinUI.Pages.AddOpenCodeGoAccountDialog;
using CliAccountSwitcher.WinUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CliAccountSwitcher.WinUI.Dialogs;

public sealed partial class AddOpenCodeGoAccountDialog : ContentDialog
{
    private readonly ApplicationThemeService _applicationThemeService = App.Services.GetRequiredService<ApplicationThemeService>();
    private readonly OpenCodeGoAccountService _openCodeGoAccountService = App.Services.GetRequiredService<OpenCodeGoAccountService>();
    private readonly AddOpenCodeGoAccountDialogContext _addOpenCodeGoAccountDialogContext;

    public AddOpenCodeGoAccountDialog()
    {
        InitializeComponent();
        _applicationThemeService.ApplyThemeToElement(this);
        _applicationThemeService.ThemeChanged += OnApplicationThemeServiceThemeChanged;
        _addOpenCodeGoAccountDialogContext = new AddOpenCodeGoAccountDialogContext(_openCodeGoAccountService, this);
        NavigateToSelectedPage();
    }

    private void NavigateToSelectedPage(bool shouldForceReload = false)
    {
        var selectedModeTag = AddAccountModeSelectorBar.SelectedItem?.Tag as string ?? "CurrentAccount";
        var selectedPageType = selectedModeTag switch
        {
            "ApiKey" => typeof(ApiKeyAddAccountPage),
            _ => typeof(CurrentAccountAddAccountPage)
        };

        if (!shouldForceReload && AddAccountContentFrame.CurrentSourcePageType == selectedPageType) return;
        AddAccountContentFrame.Navigate(selectedPageType, _addOpenCodeGoAccountDialogContext);
    }

    private void OnAddAccountModeSelectorBarSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs selectorBarSelectionChangedEventArguments) => NavigateToSelectedPage();

    private void OnApplicationThemeServiceThemeChanged(ElementTheme theme) => _applicationThemeService.ApplyThemeToElement(this);

    private void OnAddOpenCodeGoAccountDialogClosing(ContentDialog sender, ContentDialogClosingEventArgs contentDialogClosingEventArguments) => _applicationThemeService.ThemeChanged -= OnApplicationThemeServiceThemeChanged;
}
