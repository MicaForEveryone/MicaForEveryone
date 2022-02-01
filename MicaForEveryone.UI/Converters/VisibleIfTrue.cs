using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MicaForEveryone.UI.Converters
{
    public class VisibleIfTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
