using System.Globalization;

namespace MicaForEveryone.UI.Models
{
    internal class Contributor
    {
        public Contributor(string name, string gitHubUrl, string language)
        {
            Name = name;
            GitHubUrl = gitHubUrl;
            Language = language == null ? null : new CultureInfo(language);
        }

        public string Name { get; }
        public string GitHubUrl { get; }
        public CultureInfo Language { get; }
    }
}
