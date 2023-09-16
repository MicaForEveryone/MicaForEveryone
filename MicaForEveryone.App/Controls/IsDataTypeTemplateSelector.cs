#nullable disable

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace MicaForEveryone.App.Controls;

public class IsDataTypeTemplateSelector : DataTemplateSelector
{
    public Type Type { get; set; }

    public DataTemplate IsType { get; set; }

    public DataTemplate IsNotType { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is null)
            return null;

        return item.GetType().IsAssignableTo(Type) ? IsType : IsNotType;
    }
}
