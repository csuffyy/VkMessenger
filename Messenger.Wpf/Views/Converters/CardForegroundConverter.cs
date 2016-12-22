using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Messenger.Wpf.Views.Converters
{
    public class CardForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lightSkyBlue = Brushes.LightSkyBlue.Color;
            lightSkyBlue.A = (byte)(lightSkyBlue.A * 0.1);
            return (bool) value ? new SolidColorBrush(lightSkyBlue) : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}