using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Controls;

public sealed partial class SettingsPageControl : ContentControl
{
    public static DependencyProperty HeaderProperty { get; } =
        DependencyProperty.Register("Header", typeof(string), typeof(SettingsPageControl), new(null));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set
        {
            if ((string)GetValue(HeaderProperty) == value)
                return;

            SetValue(HeaderProperty, value);
        }
    }
}
