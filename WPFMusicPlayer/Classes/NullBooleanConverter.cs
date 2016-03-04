using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFMusicPlayer.Classes
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NullBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}