using System.Runtime.InteropServices;

namespace MicaForEveryone.Models;

public class SettingsModel : IEquatable<SettingsModel>
{
    public required List<IRule> Rules { get; set; }

    public bool Equals(SettingsModel? other)
    {
        if (other is null)
            return false;

        return CollectionsMarshal.AsSpan(Rules).SequenceEqual(CollectionsMarshal.AsSpan(other.Rules));
    }
}