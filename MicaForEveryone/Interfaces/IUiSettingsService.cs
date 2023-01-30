using System;
using Windows.Globalization;

namespace MicaForEveryone.Interfaces
{
    public interface IUiSettingsService
    {
        Language Language { get; set; }
        event EventHandler LanguageChanged;
    }
}