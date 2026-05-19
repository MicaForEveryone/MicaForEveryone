using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;
using MicaForEveryone.App.Helpers;

namespace MicaForEveryone.App.ViewModels;

public partial class AddClassRuleContentDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAddButtonEnabled))]
    public partial string ClassName { get; set; }

    [ObservableProperty]
    public partial IEnumerable<string>? Recommendations { get; set; }

    [ObservableProperty]
    public partial ImageSource? IconSource { get; set; }

    public bool IsAddButtonEnabled => !string.IsNullOrWhiteSpace(ClassName);

    private readonly ISettingsService _settingsService;

    public AddClassRuleContentDialogViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        ClassName = string.Empty;
    }

    public async void TryFetchIcon(HWND hwnd)
    {
        try
        {
            var icon = await IconHelper.ExtractIconFromWindowAsync(hwnd);
            if (icon != null)
            {
                var bitmapImage = IconHelper.IconToBitmapImage(icon);
                icon.Dispose();
                IconSource = bitmapImage;
            }
            else
            {
                IconSource = null;
            }
        }
        catch
        {
            IconSource = null;
        }
    }

    partial void OnClassNameChanged(string value)
    {
        IconSource = null;
    }

    [RelayCommand]
    private async Task AddRuleAsync()
    {
        var rule = new ClassRule() { ClassName = ClassName };

        if (IconSource != null)
        {
            rule.IconPath = IconHelper.GetLastExtractedPath();
        }

        _settingsService.Settings!.Rules.Insert(1, rule);
        await _settingsService.SaveAsync();
    }
}
