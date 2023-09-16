using MicaForEveryone.Models;

namespace MicaForEveryone.CoreUI;

public interface ISettingsService
{
    SettingsModel? Settings { get; set; }

    Task InitializeAsync();

    Task SaveAsync();
}
