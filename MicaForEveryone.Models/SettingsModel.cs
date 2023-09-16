using System.Runtime.InteropServices;

namespace MicaForEveryone.Models;

public class SettingsModel : IEquatable<SettingsModel>
{
    public required List<Rule> Rules { get; set; }

    // Define equal operator
    public static bool operator ==(SettingsModel? left, SettingsModel? right)
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }

    // Define not equal operator
    public static bool operator !=(SettingsModel? left, SettingsModel? right)
        => !(left == right);

    public bool Equals(SettingsModel? other)
    {
        if (other is null)
            return false;

        return CollectionsMarshal.AsSpan(Rules).SequenceEqual(CollectionsMarshal.AsSpan(other.Rules));
    }

    public override bool Equals(object? obj)
    {
        return obj is SettingsModel model && Equals(model);
    }

    public override int GetHashCode()
    {
        // Generate hash code
        return HashCode.Combine(Rules);
    }
}