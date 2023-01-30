using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Globalization;
using Windows.ApplicationModel.Resources.Core;
using Windows.System.UserProfile;

using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class LanguageService : ILanguageService
    {
        public LanguageService()
        {
            SupportedLanguages = GetSupportedLanguages().ToArray();

            var preferredLanguageTag = GlobalizationPreferences.Languages.FirstOrDefault(
                l => SupportedLanguages.Any(
                    sl => l.StartsWith(sl.LanguageTag, StringComparison.OrdinalIgnoreCase)));

            if (preferredLanguageTag == null)
            {
                CurrentLanguage = SupportedLanguages[0];
            }
            else
            {
                CurrentLanguage = SupportedLanguages.First(
                    l => preferredLanguageTag.StartsWith(l.LanguageTag, StringComparison.OrdinalIgnoreCase));
            }
        }

        public IReadOnlyList<Language> SupportedLanguages { get; }

        public Language CurrentLanguage { get; private set; }

        private IEnumerable<Language> GetSupportedLanguages()
        {
            var r = ResourceManager.Current.MainResourceMap["Resources/AppName"];
            foreach (var c in r.Candidates)
            {
                yield return new Language(c.GetQualifierValue("language"));
            }
        }

        public void SetLanguage(Language language)
        {
            CurrentLanguage = language;
            CultureInfo.CurrentCulture = new CultureInfo(language.LanguageTag);

            ResourceContext.SetGlobalQualifierValue("language", language.LanguageTag);
            ResourceContext.SetGlobalQualifierValue("layoutdirection", language.LayoutDirection switch
            {
                LanguageLayoutDirection.Ltr => "LTR",
                LanguageLayoutDirection.Rtl => "RTL",
                LanguageLayoutDirection.TtbLtr => "TTBLTR",
                LanguageLayoutDirection.TtbRtl => "TTBRTL",
                _ => throw new ArgumentOutOfRangeException(),
            });
            ResourceContext.GetForCurrentView().Reset();
            ResourceContext.GetForViewIndependentUse().Reset();
        }
    }
}
