using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MicaForEveryone.App.Helpers;

public partial class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool isNull = value is null or string { Length: 0 };
        bool invert = parameter is string p && p.Equals("invert", StringComparison.OrdinalIgnoreCase);

        if (invert)
            return isNull ? Visibility.Collapsed : Visibility.Visible;
        else
            return isNull ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}