using MicaForEveryone.Models;

namespace MicaForEveryone.CoreUI;

public interface ILocalizationService
{
    IReadOnlyList<string> SupportedLanguages { get; }

    string CurrentLanguage { get; }

    void SetLanguage(string languageTag);

    string GetLocalizedString(string key);

    string GetLocalizedTitleBarColor(TitleBarColorMode titleBarColorMode);

    string GetLocalizedBackdropType(BackdropType backdropType);

    string GetLocalizedCornerPreference(CornerPreference cornerPreference);

    string GetRuleName(Rule rule);
}
