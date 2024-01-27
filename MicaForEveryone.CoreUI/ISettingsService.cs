using MicaForEveryone.Models;
using System.ComponentModel;

namespace MicaForEveryone.CoreUI;

public interface ISettingsService : INotifyPropertyChanged
{
    RulesModel? Settings { get; set; }

    Task InitializeAsync();

    Task SaveAsync();
}
