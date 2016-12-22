using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Messenger.Wpf.Views.Converters
{
    public class XYToPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var t = values[0].GetType();
            var coord = values.Select(v => (double) v).ToList();
            var point = new Point((int) (coord[0]/2), (int) (coord[1]/2));
            return point;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}