using System.Collections.Generic;
using Windows.Globalization;

namespace MicaForEveryone.Interfaces
{
    public interface ILanguageService
    {
        IReadOnlyList<Language> SupportedLanguages { get; }

        Language CurrentLanguage { get; }

        void SetLanguage(Language language);
    }
}
