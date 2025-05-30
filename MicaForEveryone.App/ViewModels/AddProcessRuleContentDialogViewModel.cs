using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MicaForEveryone.App.ViewModels;

public partial class AddProcessRuleContentDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAddButtonEnabled))]
    public partial string ProcessName { get; set; }

    public string FilePath { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Process> Recommendations { get; set; }

    public bool IsAddButtonEnabled => !string.IsNullOrWhiteSpace(ProcessName);

    private readonly ISettingsService _settingsService;

    public AddProcessRuleContentDialogViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        ProcessName = string.Empty;
        FilePath = string.Empty;
        Recommendations = new ObservableCollection<Process>();
    }

    public void RequestSuggestions()
    {
        if (string.IsNullOrWhiteSpace(ProcessName))
        {
            Recommendations.Clear();
            return;
        }

        IEnumerable<Process> newRecommendations = Process
            .GetProcesses()
            .Where(f => f.ProcessName.StartsWith(ProcessName, System.StringComparison.CurrentCultureIgnoreCase))
            .Distinct(ProcessNameComparer.Instance);
        IEnumerable<Process> toRemove = Recommendations.Except(newRecommendations).ToArray();
        IEnumerable<Process> toAdd = newRecommendations.Except(Recommendations).ToArray();

        foreach (Process recommendationToRemove in toRemove)
        {
            Recommendations.Remove(recommendationToRemove);
        }

        foreach (Process recommendationToAdd in toAdd)
        {
            Recommendations.Add(recommendationToAdd);
        }
    }

    [RelayCommand]
    private async Task AddRuleAsync()
    {
        _settingsService.Settings!.Rules.Insert(1, new ProcessRule()
        {
            ProcessName = ProcessName,
            FilePath = FilePath
        });
        await _settingsService.SaveAsync();
    }
}

internal class ProcessNameComparer : IEqualityComparer<Process>
{
    public bool Equals(Process? x, Process? y)
    {
        return x is not null && y is not null && x.ProcessName == y.ProcessName;
    }

    public int GetHashCode([DisallowNull] Process obj)
    {
        return obj.ProcessName.GetHashCode();
    }

    public static readonly ProcessNameComparer Instance = new();
}
