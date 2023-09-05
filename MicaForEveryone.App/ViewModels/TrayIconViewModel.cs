using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.App.Service;

namespace MicaForEveryone.App.ViewModels;

public partial class TrayIconViewModel : ObservableObject
{
    private readonly MainAppService _mainAppService;

    public TrayIconViewModel(MainAppService mainAppService)
    {
        _mainAppService = mainAppService;
    }

    [RelayCommand]
    private void Exit()
        => _mainAppService.Shutdown();

    [RelayCommand]
    private void Settings()
        => _mainAppService.ActivateSettings();
}
