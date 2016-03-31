using System;
using System.Windows;
using System.Windows.Data;

namespace WPFMusicPlayer.Classes
{
    [ValueConversion(typeof(bool), typeof(int))]
    public class NetworkAvalibilityToBlurEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean && (bool)value)
            {
                return 10;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Int32 && (Int32)value == 10)
            {
                return true;
            }
            return false;
        }
    }
}