using Microsoft.UI.Xaml.Data;
using System;

namespace MicaForEveryone.App.Helpers;

public partial class MultiplyConverter : IValueConverter
{
    public double Multiplier { get; set; } = 1.0;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double d)
            return d * Multiplier;
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}