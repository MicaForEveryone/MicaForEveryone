using MicaForEveryone.Models;
using System;

namespace MicaForEveryone.App.Helpers;

public static class EnumHelper
{
    private static string[]? _titleBarColorModes;

    public static string[] TitleBarColorModes
    {
        get
        {
            return _titleBarColorModes ??= Enum.GetNames<TitleBarColorMode>();
        }
    }

    public static TitleBarColorMode StringToTitleBarColorMode(object value)
    {
        return Enum.Parse<TitleBarColorMode>((string)value);
    }

    public static string TitleBarColorModeToString(TitleBarColorMode t)
        => t.ToString();
}