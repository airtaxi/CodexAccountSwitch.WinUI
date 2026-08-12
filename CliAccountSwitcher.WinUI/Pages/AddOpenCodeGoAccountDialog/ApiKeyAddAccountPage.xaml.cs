using CliAccountSwitcher.Api.Providers.OpenCodeGo.Models;
using CliAccountSwitcher.WinUI.Dialogs;
using CliAccountSwitcher.WinUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace CliAccountSwitcher.WinUI.Pages.AddOpenCodeGoAccountDialog;

public sealed partial class ApiKeyAddAccountPage : Page
{
    private readonly LocalizationService _localizationService = App.Services.GetRequiredService<LocalizationService>();
    private AddOpenCodeGoAccountDialogContext _addAccountDialogContext;
    private bool _isCompletingSuccessfully;

    public ApiKeyAddAccountPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments) => _addAccountDialogContext = navigationEventArguments.Parameter as AddOpenCodeGoAccountDialogContext;

    private async void OnAddApiKeyButtonClicked(object sender, RoutedEventArgs routedEventArguments)
    {
        if (_addAccountDialogContext is null) return;

        var apiKey = ApiKeyPasswordBox.Password;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            ShowError(_localizationService.GetLocalizedString("OpenCodeGoApiKeyAddAccountPage_ErrorInfoBar.Message"), _localizationService.GetLocalizedString("OpenCodeGoApiKeyAddAccountPage_ErrorInfoBar.Title"));
            return;
        }

        ErrorInfoBar.IsOpen = false;
        ValidationProgressRing.IsActive = true;
        ValidationProgressRing.Visibility = Visibility.Visible;
        _addAccountDialogContext.SetInteractionEnabled(false);

        try
        {
            await _addAccountDialogContext.OpenCodeGoAccountService.AddApiKeyAsync(apiKey, AliasTextBox.Text);
            _isCompletingSuccessfully = true;
            _addAccountDialogContext.CompleteSuccessfully();
        }
        catch (OpenCodeGoSubscriptionRequiredException)
        {
            ShowError(_localizationService.GetLocalizedString("OpenCodeGoApiKeyAddAccountPage_ErrorInfoBar_SubscriptionRequiredMessage"), _localizationService.GetLocalizedString("OpenCodeGoApiKeyAddAccountPage_ErrorInfoBar_SubscriptionRequiredTitle"));
        }
        catch
        {
            ShowError(_localizationService.GetLocalizedString("OpenCodeGoApiKeyAddAccountPage_ErrorInfoBar.Message"), _localizationService.GetLocalizedString("OpenCodeGoApiKeyAddAccountPage_ErrorInfoBar.Title"));
        }
        finally
        {
            if (!_isCompletingSuccessfully) _addAccountDialogContext.SetInteractionEnabled(true);
            ValidationProgressRing.IsActive = false;
            ValidationProgressRing.Visibility = Visibility.Collapsed;
        }
    }

    private void ShowError(string message, string title)
    {
        ErrorInfoBar.Message = message;
        ErrorInfoBar.Title = title;
        ErrorInfoBar.IsOpen = true;
    }
}
