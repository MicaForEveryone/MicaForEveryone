using Microsoft.UI.Xaml.Data;
using System;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace MicaForEveryone.App.Helpers;

public partial class ColorConverter : IValueConverter
{
    public static Color ConvertToColor(string? hex)
    {
        if (string.IsNullOrEmpty(hex))
            hex = "#0078D4";

        ReadOnlySpan<char> spanHex = hex;

        // Remove # if present
        if (hex.StartsWith("#"))
            spanHex = spanHex.Slice(1);

        Span<byte> bytes = stackalloc byte[3];
        System.Convert.FromHexString(spanHex, bytes, out int _, out int _);
        return Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        string? hex = value as string;
        return ConvertToColor(hex);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        Color color = (Color)value;
        Span<char> buffer = stackalloc char[7];
        DefaultInterpolatedStringHandler interpolation = new(1, 1, null, buffer);
        interpolation.AppendLiteral("#");
        interpolation.AppendFormatted(System.Convert.ToHexString([color.R, color.G, color.B]));
        return interpolation.ToString();
    }
}