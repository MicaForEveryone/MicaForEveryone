using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Core.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    public class UiSettingsServiceService : IUiSettingsService
    {
        private const string LanguageKey = "language";

        private readonly ILanguageService _languageService;

        private readonly ISettingsContainer _container;

        public UiSettingsServiceService(ISettingsContainer container, ILanguageService languageService)
        {
            _container = container;
            _languageService = languageService;
        }

        public Language Language
        {
            get => _languageService.CurrentLanguage;
            set
            {
                if (_languageService.CurrentLanguage == value)
                    return;

                _languageService.SetLanguage(value);
                _container.SetValue(LanguageKey, _languageService.CurrentLanguage.LanguageTag);

                LanguageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Load()
        {
            var languageTag = _container.GetValue(LanguageKey) as string;
            var language = _languageService.SupportedLanguages.FirstOrDefault(
                                language => language.LanguageTag == languageTag);
            if (language != null)
            {
                _languageService.SetLanguage(language);
            }
        }

        public event EventHandler? LanguageChanged;
    }
}
