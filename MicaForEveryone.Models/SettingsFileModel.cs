using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MicaForEveryone.Models;

public partial class SettingsFileModel : ObservableObject, IEquatable<SettingsFileModel>
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
    public static bool operator ==(SettingsFileModel? left, SettingsFileModel? right)
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }

    // Define not equal operator
    public static bool operator !=(SettingsFileModel? left, SettingsFileModel? right)
        => !(left == right);

    public bool Equals(SettingsFileModel? other)
    {
        if (other is null)
            return false;

        return Enumerable.SequenceEqual(Rules, other.Rules);
    }

    public override bool Equals(object? obj)
    {
        return obj is SettingsFileModel model && Equals(model);
    }

    public override int GetHashCode()
    {
        // Generate hash code
        return HashCode.Combine(Rules);
    }
}