using CliAccountSwitcher.WinUI.Services;
using Microsoft.UI.Xaml.Controls;

namespace CliAccountSwitcher.WinUI.Dialogs;

public sealed class AddOpenCodeGoAccountDialogContext(OpenCodeGoAccountService openCodeGoAccountService, ContentDialog contentDialog)
{
    public OpenCodeGoAccountService OpenCodeGoAccountService { get; } = openCodeGoAccountService;

    public ContentDialog ContentDialog { get; } = contentDialog;

    public void SetInteractionEnabled(bool isInteractionEnabled) => ContentDialog.IsEnabled = isInteractionEnabled;

    public void CompleteSuccessfully() => ContentDialog.Hide();
}
