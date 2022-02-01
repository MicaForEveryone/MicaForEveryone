using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MicaForEveryone.UI.Converters
{
    public class VisibleIfFalse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool) value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
