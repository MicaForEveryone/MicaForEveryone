using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicaForEveryone.App.Helpers;

/// <summary>
/// Convert exe file path to BitmapImage.
/// </summary>
public partial class FileIconToBitmapImageConverter : IValueConverter
{
    /// <summary>
    /// Convert exe file path to BitmapImage.
    /// </summary>
    /// <param name="value">exe file path</param>
    /// <returns>BitmapImage</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is null)
        {
            return DefaultIcon;
        }
        string path = (string) value;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return DefaultIcon;
        }
        Icon? icon = Icon.ExtractAssociatedIcon(path);
        if (icon is null)
        {
            return DefaultIcon;
        }
        return IconToBitmapImage(icon);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    private static BitmapImage IconToBitmapImage(Icon icon)
    {
        using MemoryStream stream = new();
        icon.ToBitmap().Save(stream, ImageFormat.Png);
        stream.Position = 0;
        BitmapImage bitmapImage = new();
        bitmapImage.SetSource(stream.AsRandomAccessStream());
        return bitmapImage;
    }

    /// <summary>
    /// Show this icon when target icon is not available.
    /// <br/>
    /// For example, exception thrown when get Process.MainModule.
    /// </summary>
    private static BitmapImage DefaultIcon
    {
        get
        {
            field ??= new BitmapImage(new Uri("ms-appx:///Assets/LockScreenLogo.scale-200.png"));
            return field;
        }
    }
}
