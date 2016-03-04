using System;
using System.Windows;
using System.Windows.Data;

namespace WPFMusicPlayer.Classes
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class ValueToVisibilityConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Int32 && (int)value>0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}