using System;
using Windows.UI.Xaml.Data;

namespace MicaForEveryone.UI.Converters
{
    internal class IsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
