using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MicaForEveryone.UI.Converters
{
    internal class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string s)
            {
                return Enum.Parse(targetType, s);
            }
            else if (value is ComboBoxItem item)
            {
                return ConvertBack(item.Content, targetType, parameter, language);
            }
            else
            {
                throw new Exception("unhandled type " + value.GetType());
            }
        }
    }
}
