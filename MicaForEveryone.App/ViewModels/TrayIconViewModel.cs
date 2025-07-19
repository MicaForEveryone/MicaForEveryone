using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.App.Services;
using MicaForEveryone.CoreUI;

namespace MicaForEveryone.App.ViewModels;

public sealed partial class TrayIconViewModel : ObservableObject
{
    private readonly MainAppService _mainAppService;
    private readonly IRuleService _ruleService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleEffectsText))]
    private bool _areEffectsEnabled = true;

    public string ToggleEffectsText => AreEffectsEnabled ? "Disable All Effects" : "Enable All Effects";

    public TrayIconViewModel(MainAppService mainAppService, IRuleService ruleService)
    {
        _mainAppService = mainAppService;
        _ruleService = ruleService;
        AreEffectsEnabled = _ruleService.AreEffectsEnabled;
    }

    [RelayCommand]
    private void Exit()
        => _mainAppService.Shutdown();

    [RelayCommand]
    private void Settings()
        => _mainAppService.ActivateSettings();

    [RelayCommand]
    private void ToggleEffects()
    {
        _ruleService.AreEffectsEnabled = !_ruleService.AreEffectsEnabled;
        AreEffectsEnabled = _ruleService.AreEffectsEnabled;
        _ruleService.ApplyRulesToAllWindowsAsync();
    }
}
