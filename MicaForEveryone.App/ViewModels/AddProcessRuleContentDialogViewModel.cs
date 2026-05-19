using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;
using MicaForEveryone.App.Helpers;

namespace MicaForEveryone.App.ViewModels;

public partial class AddProcessRuleContentDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAddButtonEnabled))]
    public partial string ProcessName { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> Recommendations { get; set; }

    [ObservableProperty]
    public partial ImageSource? IconSource { get; set; }

    public bool IsAddButtonEnabled => !string.IsNullOrWhiteSpace(ProcessName);

    private readonly ISettingsService _settingsService;

    public AddProcessRuleContentDialogViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        ProcessName = string.Empty;
        Recommendations = new ObservableCollection<string>();
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

    partial void OnProcessNameChanged(string value)
    {
        IconSource = null;
    }

    public void RequestSuggestions()
    {
        if (string.IsNullOrWhiteSpace(ProcessName))
        {
            Recommendations.Clear();
            return;
        }

        IEnumerable<string> newRecommendations = Process
            .GetProcesses()
            .Select(f => f.ProcessName)
            .Where(f => f.StartsWith(ProcessName, System.StringComparison.CurrentCultureIgnoreCase))
            .Distinct();
        IEnumerable<string> toRemove = Recommendations.Except(newRecommendations).ToArray();
        IEnumerable<string> toAdd = newRecommendations.Except(Recommendations).ToArray();

        foreach (string recommendationToRemove in toRemove)
        {
            Recommendations.Remove(recommendationToRemove);
        }

        foreach (string recommendationToAdd in toAdd)
        {
            Recommendations.Add(recommendationToAdd);
        }
    }

    [RelayCommand]
    private async Task AddRuleAsync()
    {
        var rule = new ProcessRule() { ProcessName = ProcessName };

        if (IconSource != null)
        {
            rule.IconPath = IconHelper.GetLastExtractedPath();
        }

        _settingsService.Settings!.Rules.Insert(1, rule);
        await _settingsService.SaveAsync();
    }
}
