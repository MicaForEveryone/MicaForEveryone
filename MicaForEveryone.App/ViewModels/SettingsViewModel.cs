using CommunityToolkit.Mvvm.ComponentModel;
using MicaForEveryone.CoreUI;

namespace MicaForEveryone.App.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    public ISettingsService SettingsService { get; }

    public SettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;
    }
}