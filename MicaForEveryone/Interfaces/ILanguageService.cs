using System;
using System.Collections.Generic;
using System.Text;
using Windows.Globalization;

namespace MicaForEveryone.Interfaces
{
    public interface ILanguageService
    {
        Language[] SupportedLanguages { get; }

        Language CurrentLanguage { get; }

        void SetLanguage(Language language);
    }
}
