using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace WPFMusicPlayer.ViewModel
{
    public class SettingsFlyoutViewModel :ViewModelBase
    {
        public IEnumerable<Swatch> Swatches { get; }
        public MainViewModel MainVm { get; }

        public SettingsFlyoutViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;
            MainVm = ((MainViewModel)Application.Current.MainWindow.DataContext);
        }

        private RelayCommand<object> _applyPrimaryColorCommand;
        public RelayCommand<object> ApplyPrimaryColor
        {
            get
            {
                return _applyPrimaryColorCommand
                    ?? (_applyPrimaryColorCommand = new RelayCommand<object>(
                    (swatch) =>
                    {
                        new PaletteHelper().ReplacePrimaryColor(swatch as Swatch);
                        MainVm.Settings.PrimaryColor = ((Swatch)swatch).Name;
                    }));
            }
        }

        private RelayCommand<object> _applyAccentColorCommand;
        public RelayCommand<object> ApplyAccentColor
        {
            get
            {
                return _applyAccentColorCommand
                    ?? (_applyAccentColorCommand = new RelayCommand<object>(
                    (swatch) =>
                    {
                        
                        new PaletteHelper().ReplaceAccentColor(swatch as Swatch);
                        MainVm.Settings.AccentColor= ((Swatch)swatch).Name;
                    }));
            }
        }

        private RelayCommand<bool> _changeLightDarkThemeCommand;
        public RelayCommand<bool> ChangeLightDarkTheme
        {
            get
            {
                return _changeLightDarkThemeCommand
                    ?? (_changeLightDarkThemeCommand = new RelayCommand<bool>(
                    (isDark) =>
                    {
                        new PaletteHelper().SetLightDark(isDark);
                    }));
            }
        }
    }
}