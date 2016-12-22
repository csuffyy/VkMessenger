using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Messenger.Wpf.Views.Converters
{
    public class CardAlignmentConverter : IValueConverter
    {
        private static string previous;
        private static HorizontalAlignment prevAlignment;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                prevAlignment = HorizontalAlignment.Left;
                return HorizontalAlignment.Left;
            }
                
            HorizontalAlignment align;
            if (Equals(previous, value))
                align = prevAlignment;
            else
            {
                align =
                    prevAlignment == HorizontalAlignment.Left
                    ? HorizontalAlignment.Right
                    : HorizontalAlignment.Left;
            }

            previous = (string)value;
            prevAlignment = align;
            return align;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}