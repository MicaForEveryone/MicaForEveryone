using MicaForEveryone.App.ViewModels;
using MicaForEveryone.CoreUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System.Threading.Tasks;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Globalization;
using Windows.ApplicationModel.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AppSettingsPage : Page
{
    private AppSettingsPageViewModel ViewModel { get; }
    private IStartupService StartupService { get; }

    private ISettingsService SettingsService { get; }
    private ILocalizationService LocalizationService { get; }
    private bool unloaded;

    public AppSettingsPage()
    {
        this.InitializeComponent();

        ViewModel = App.Services.GetRequiredService<AppSettingsPageViewModel>();
        StartupService = App.Services.GetRequiredService<IStartupService>();
        SettingsService = App.Services.GetRequiredService<ISettingsService>();
        LocalizationService = App.Services.GetRequiredService<ILocalizationService>();

        PopulateLanguages();
        _ = PopulateStartupToggle();
    }

    private void PopulateLanguages()
    {
        var systemItem = new ComboBoxItem
        {
            Content = LocalizationService.GetLocalizedString("SystemLanguageName"),
            Tag = string.Empty
        };
        LanguageComboBox.Items.Add(systemItem);
        LanguageComboBox.SelectedItem = systemItem;

        foreach (string tag in LocalizationService.SupportedLanguages)
        {
            var item = new ComboBoxItem { Content = new Language(tag).NativeName, Tag = tag };
            LanguageComboBox.Items.Add(item);
            if (string.Equals(tag, LocalizationService.CurrentLanguage, StringComparison.OrdinalIgnoreCase))
                LanguageComboBox.SelectedItem = item;
        }
        LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
    }

    private async void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LanguageComboBox.SelectedItem is ComboBoxItem { Tag: string languageTag }
            && !string.Equals(languageTag, LocalizationService.CurrentLanguage, StringComparison.OrdinalIgnoreCase))
        {
            LanguageComboBox.IsEnabled = false;
            bool languageSaved = false;
            bool failed = false;
            try
            {
                // Let existing rule edits finish without rewriting settings just to change language.
                await SettingsService.WaitForPendingSaveAsync();
                if (unloaded)
                    return;
                LocalizationService.SetLanguage(languageTag);
                languageSaved = true;
                // WinUI caches x:Uid resources. Restart so every surface uses the new language.
                failed = AppInstance.Restart("--settings") != AppRestartFailureReason.RestartPending;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or COMException or Win32Exception)
            {
                Debug.WriteLine(ex);
                failed = true;
            }
            finally
            {
                if (!languageSaved && e.RemovedItems.Count > 0)
                {
                    LanguageComboBox.SelectionChanged -= LanguageComboBox_SelectionChanged;
                    LanguageComboBox.SelectedItem = e.RemovedItems[0];
                    if (!unloaded)
                        LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
                }
                LanguageComboBox.IsEnabled = true;
            }

            if (failed && !unloaded && XamlRoot is not null)
            {
                ContentDialog dialog = new()
                {
                    XamlRoot = XamlRoot,
                    Title = LocalizationService.GetLocalizedString(languageSaved ? "LanguageRestartErrorTitle" : "LanguageChangeErrorTitle"),
                    Content = LocalizationService.GetLocalizedString(languageSaved ? "LanguageRestartErrorMessage" : "LanguageChangeErrorMessage"),
                    CloseButtonText = LocalizationService.GetLocalizedString("CancelButton/Content")
                };
                await dialog.ShowAsync();
            }
        }
    }

    private async Task PopulateStartupToggle()
    {
        StartupToggle.IsEnabled = await StartupService.GetStartupAvailableAsync();
        StartupToggle.IsOn = await StartupService.GetStartupEnabledAsync();
    }

    private async void StartupToggle_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (StartupToggle.IsOn != await StartupService.GetStartupEnabledAsync())
        {
            await StartupService.SetStartupEnabledAsync(StartupToggle.IsOn);
            await PopulateStartupToggle();
        }
    }

    private void TelemetryToggle_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _ = SettingsService.SaveAsync();
    }

    private void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        unloaded = true;
        Unloaded -= Page_Unloaded;
        StartupToggle.Toggled -= StartupToggle_Toggled;
        TelemetryToggle.Toggled -= TelemetryToggle_Toggled;
        LanguageComboBox.SelectionChanged -= LanguageComboBox_SelectionChanged;

        Bindings?.StopTracking();
    }
}
