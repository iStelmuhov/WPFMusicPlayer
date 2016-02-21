using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using VkNet.Enums.Filters;
using WPFMusicPlayer.Classes;
using User = VkNet.Model.User;

namespace WPFMusicPlayer.ViewModel
{
    public class UserProfilerViewModel :ViewModelBase
    {
        private string pxApiKey= "rEDd7OkHtWg6twlvw1fxyUTqLRXq2HVxVA1RVrQG";


        public const string BackgroundImagePropertyName = "BackgroundImage";
        private WebImage _backgroundImage;
        public WebImage BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }

            set
            {
                if (_backgroundImage == value)
                {
                    return;
                }

                _backgroundImage = value;
                RaisePropertyChanged(BackgroundImagePropertyName);
            }
        }


        public const string UserInformationPropertyName = "UserInformation";
        private User _userInformation;
        public User UserInformation
        {
            get
            {
                return _userInformation;
            }

            set
            {
                if (_userInformation == value)
                {
                    return;
                }

                _userInformation = value;
                RaisePropertyChanged(UserInformationPropertyName);
            }
        }

        private int _currentPage;
        private int _currentPhotoNumber;

        public MainViewModel MainVm { get; set; }

        public UserProfilerViewModel()
        {
            MainVm = ((MainViewModel)Application.Current.MainWindow.DataContext);

            _currentPage = 1;
            _currentPhotoNumber = 1;

            LoadBgImage();
            LoadUserInformation();
        }

        private async void LoadBgImage()
        {
            BackgroundImage = await WebImage.GetMostPopularPhotoFrom500px(pxApiKey, 21,_currentPage,_currentPhotoNumber++);
        }

        private void LoadUserInformation()
        {
            if (MainVm.VkApi.UserId != null)
                UserInformation = MainVm.VkApi.Users.Get((long) MainVm.VkApi.UserId,ProfileFields.Photo200);
        }


        private RelayCommand _refreshBgImageCommand;
        public RelayCommand RefreshBgImage
        {
            get
            {
                return  _refreshBgImageCommand
                    ?? ( _refreshBgImageCommand = new RelayCommand(async () =>
                    {
                        if (_currentPhotoNumber == 1000)
                        {
                            _currentPage += 1;
                            _currentPhotoNumber = 0;
                        }

                        BackgroundImage = await WebImage.GetMostPopularPhotoFrom500px(pxApiKey, 21, _currentPage, _currentPhotoNumber++);

                    }));
            }
        }

        private RelayCommand _openSettingsFlyoutCommand;

        /// <summary>
        /// Gets the OpenSettingsFlyout.
        /// </summary>
        public RelayCommand OpenSettingsFlyout
        {
            get
            {
                return _openSettingsFlyoutCommand
                    ?? (_openSettingsFlyoutCommand = new RelayCommand(
                    () =>
                    {
                        ((MainWindow) Application.Current.MainWindow).LeftFlyout.IsOpen = true;
                    }));
            }
        }

    }
}