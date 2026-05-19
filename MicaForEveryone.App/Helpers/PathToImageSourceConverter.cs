using Microsoft.UI.Xaml.Data;
using System;
using System.IO;

namespace MicaForEveryone.App.Helpers;

public partial class PathToImageSourceConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string path && !string.IsNullOrEmpty(path) && File.Exists(path))
        {
            try
            {
                using var icon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                if (icon != null)
                    return IconHelper.IconToBitmapImage(icon);
            }
            catch
            {
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}