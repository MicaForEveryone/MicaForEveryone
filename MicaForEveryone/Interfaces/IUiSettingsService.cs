using System;
using System.Threading.Tasks;
using Windows.Globalization;

namespace MicaForEveryone.Interfaces
{
    public interface IUiSettingsService
    {
        void Load();

        Language Language { get; set; }
        event EventHandler LanguageChanged;
    }
}