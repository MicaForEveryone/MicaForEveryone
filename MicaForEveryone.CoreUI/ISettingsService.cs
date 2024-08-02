using MicaForEveryone.Models;
using System.ComponentModel;

namespace MicaForEveryone.CoreUI;

public interface ISettingsService : INotifyPropertyChanged, IDisposable
{
    SettingsFileModel? Settings { get; set; }

    Task InitializeAsync();

    Task SaveAsync();
}
