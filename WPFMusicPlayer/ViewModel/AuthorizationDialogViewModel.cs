using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using VkNet;
using VkNet.Enums.Filters;
using WPFMusicPlayer.Views;

namespace WPFMusicPlayer.ViewModel
{
    public class AuthorizationDialogViewModel:ViewModelBase
    {
        private MainViewModel MainVm { get; }

       public AuthorizationDialogViewModel()
        {
            MainVm = ((MainViewModel)Application.Current.MainWindow.DataContext);
        }

        public const string LoginPropertyName = "Login";
        private string _login;
        public string Login
        {
            get
            {
                return _login;
            }

            set
            {
                if (_login == value)
                {
                    return;
                }

                _login = value;
                RaisePropertyChanged(LoginPropertyName);
            }
        }

        public bool RememberData { get; set; }

        private RelayCommand<PasswordBox> _tryAuthorizеCommand;
        public RelayCommand<PasswordBox> TryAuthorize
        {
            get
            {
                return _tryAuthorizеCommand
                    ?? (_tryAuthorizеCommand = new RelayCommand<PasswordBox>(async passwordBox =>
                    {
                        try
                        {
                            MainVm.ShowProgressBar = true;
                            var autorizeAuthParams = new ApiAuthParams
                            {

                                Login = Login,
                                Password = passwordBox.Password,
                                ApplicationId = MainViewModel.Appid,
                                Settings = Settings.All
                            };

                             MainVm.VkApi.Authorize(autorizeAuthParams);

                            if (RememberData)
                            {
                                MainVm.Settings.Login = autorizeAuthParams.Login;
                                MainVm.Settings.Password = autorizeAuthParams.Password;
                                MainVm.Settings.SaveLoginPassword = true;
                            }

                            MainVm.OnAuthorizationSuccess();
                            MainVm.IsHostDialogOpen = false;
                            MainVm.AuthorizationPanel=new UserProfiler();
                            MainVm.ShowProgressBar = false;
                        }
                        catch (Exception ex)
                        {
                            var materialSettings = new MetroDialogSettings
                            {
                                CustomResourceDictionary =
                                        new ResourceDictionary()
                                        {
                                            Source =
                                                new Uri(
                                                    "pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
                                        },
                                SuppressDefaultResources = true,
                                AnimateShow = true,
                                ColorScheme = MetroDialogColorScheme.Accented
                            };
                            MainVm.IsHostDialogOpen = false;
                            await DialogCoordinator.Instance.ShowMessageAsync(this, "Ошибка", ex.Message, MessageDialogStyle.Affirmative, materialSettings);

                            MainVm.ShowProgressBar = false;
                        }
                    }));
            }
        }

    }
}