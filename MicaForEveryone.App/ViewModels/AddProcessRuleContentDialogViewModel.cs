using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MicaForEveryone.App.ViewModels;

public partial class AddProcessRuleContentDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAddButtonEnabled))]
    private string _processName = string.Empty;

    [ObservableProperty]
    private IEnumerable<string>? _recommendations;

    public bool IsAddButtonEnabled => !string.IsNullOrWhiteSpace(ProcessName);

    private readonly ISettingsService _settingsService;

    public AddProcessRuleContentDialogViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void RequestSuggestions()
    {
        if (string.IsNullOrWhiteSpace(ProcessName))
        {
            Recommendations = Enumerable.Empty<string>();
            return;
        }

        Recommendations =
            Process
            .GetProcesses()
            .Select(f => f.ProcessName)
            .Where(f => f.StartsWith(ProcessName, System.StringComparison.CurrentCultureIgnoreCase))
            .Distinct();
    }

    [RelayCommand]
    private async Task AddRuleAsync()
    {
        _settingsService.Settings!.Rules.Insert(1, new ProcessRule() { ProcessName = ProcessName });
        await _settingsService.SaveAsync();
    }
}
