using System;
using System.Globalization;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFMusicPlayer.Classes
{
    [ValueConversion(typeof(bool),typeof(bool))]
    public class SelectedAudioControlConventer:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value && ((MainWindow)Application.Current.MainWindow).CurrentListItem.Content.GetType().Name == parameter.ToString())
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}