using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MicaForEveryone.Models;

public class SettingsModel : ObservableObject, IEquatable<SettingsModel>
{
    private ObservableCollection<Rule>? _rules;

    public required ObservableCollection<Rule> Rules
    {
        get => _rules!;
        set
        {
            if (_rules is not null && Enumerable.SequenceEqual(_rules, value))
                return;

            _rules = value;
            OnPropertyChanged(nameof(Rules));
        }
    }

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

        return Enumerable.SequenceEqual(Rules, other.Rules);
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