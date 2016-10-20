using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using WPFMusicPlayer.Views;

namespace WPFMusicPlayer.ViewModel
{
    public class AuthorizationControlViewModel:ViewModelBase
    {
        private MainViewModel MainVm { get; set; }
    
        public AuthorizationControlViewModel()
        {
            MainVm = ((MainViewModel)Application.Current.MainWindow.DataContext);
        }

        private RelayCommand _userAuthorizationCommand;
        public RelayCommand UserAuthorization
        {
            get
            {
                return  _userAuthorizationCommand
                    ?? ( _userAuthorizationCommand = new RelayCommand(() =>
                    {
                        if (MainVm == null)
                        {
                            MainVm = ((MainViewModel)Application.Current.MainWindow.DataContext);
                        }

                        MainVm.DialogHostContent = new AuthorizationDialog();
                        MainVm.IsHostDialogOpen = true;
                    }));
            }
        }



    }
}