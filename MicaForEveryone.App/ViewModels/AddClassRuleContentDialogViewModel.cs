using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MicaForEveryone.App.ViewModels;

public partial class AddClassRuleContentDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAddButtonEnabled))]
    private string _className = string.Empty;

    [ObservableProperty]
    private IEnumerable<string>? _recommendations;

    public bool IsAddButtonEnabled => !string.IsNullOrWhiteSpace(ClassName);

    private readonly ISettingsService _settingsService;

    public AddClassRuleContentDialogViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [RelayCommand]
    private async Task AddRuleAsync()
    {
        _settingsService.Settings!.Rules.Insert(1, new ClassRule() { ClassName = ClassName });
        await _settingsService.SaveAsync();
    }
}
