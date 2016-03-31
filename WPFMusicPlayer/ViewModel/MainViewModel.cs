using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using VkNet;
using WPFMusicPlayer.Classes;
using WPFMusicPlayer.Views;

namespace WPFMusicPlayer.ViewModel
{ 
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public  static ulong Appid = 5233775;

        public VkApi VkApi;

        private DispatcherTimer _timer;

        public const string MePlayerPropertyName = "MePlayer";
        private AudioFile _mePlayerFile = new AudioFile();
        public AudioFile MePlayer
        {
            get
            {
                return _mePlayerFile;
            }

            set
            {
                if (_mePlayerFile == value)
                {
                    return;
                }

                _mePlayerFile = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(MePlayerPropertyName);
            }
        }


        public const string RepeatAudioPropertyName = "RepeatAudio";
        private bool _repeatAudio;
        public bool RepeatAudio
        {
            get
            {
                return _repeatAudio;
            }

            set
            {
                if (_repeatAudio == value)
                {
                    return;
                }

                _repeatAudio = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(RepeatAudioPropertyName);
            }
        }


        public const string ShufflePropertyName = "Shuffle";
        private bool _shuffle;
        public bool Shuffle
        {
            get
            {
                return _shuffle;
            }

            set
            {
                if (_shuffle == value)
                {
                    return;
                }

                _shuffle = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(ShufflePropertyName);
            }
        }


        public const string IsHostDialogOpenPropertyName = "IsHostDialogOpen";
        private bool _osHostDialogOpen;
        public bool IsHostDialogOpen
        {
            get
            {
                return _osHostDialogOpen;
            }

            set
            {
                if (_osHostDialogOpen == value)
                {
                    return;
                }

                _osHostDialogOpen = value;
                RaisePropertyChanged(IsHostDialogOpenPropertyName);
            }
        }


        public const string DialogHostContentPropertyName = "DialogHostContent";
        private object _dialogHostContent;
        public object DialogHostContent
        {
            get
            {
                return _dialogHostContent;
            }

            set
            {
                if (_dialogHostContent == value)
                {
                    return;
                }

                _dialogHostContent = value;
                RaisePropertyChanged(DialogHostContentPropertyName);
            }
        }

        public const string AuthorizationPanelPropertyName = "AuthorizationPanel";
        private object _authorizationPanel = false;
        public object AuthorizationPanel
        {
            get
            {
                return _authorizationPanel;
            }

            set
            {
                if (_authorizationPanel == value)
                {
                    return;
                }

                _authorizationPanel = value;
                RaisePropertyChanged(AuthorizationPanelPropertyName);
            }
        }


        public const string ShowProgressBarPropertyName = "ShowProgressBar";
        private bool _showProgressBar;
        public bool ShowProgressBar
        {
            get
            {
                return _showProgressBar;
            }

            set
            {
                if (_showProgressBar == value)
                {
                    return;
                }

                _showProgressBar = value;
                RaisePropertyChanged(ShowProgressBarPropertyName);
            }
        }


        public const string InternetConnectionAvailabilityPropertyName = "InternetConnectionAvailability";
        private bool _internetConnectionAvalibility = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        public bool InternetConnectionAvailability
        {
            get
            {
                return _internetConnectionAvalibility;
            }

            set
            {
                if (_internetConnectionAvalibility == value)
                {
                    return;
                }

                _internetConnectionAvalibility = value;
                RaisePropertyChanged(InternetConnectionAvailabilityPropertyName);
            }
        }

        private DispatcherTimer _checkNetworkAvailability;

        public ProgramSettings Settings { get; private set; }

        #endregion

        public MainViewModel()
        {

            try
            {
                using (StreamReader sr=new StreamReader("settings.data"))
                {
                    string jsonObj = sr.ReadToEnd();
                    Settings = JsonConvert.DeserializeObject<ProgramSettings>(jsonObj);
                }
                
            }
            catch (Exception)
            {
                Settings=new ProgramSettings();
            }

            var paletteHelper= new PaletteHelper();
            
            paletteHelper.ReplacePrimaryColor(ProgramSettings.FindSwatchByName(Settings.PrimaryColor));
            paletteHelper.ReplaceAccentColor(ProgramSettings.FindSwatchByName(Settings.AccentColor));
            paletteHelper.SetLightDark(Settings.IsDark);


            VkApi =new VkApi();

            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timer.Tick += _timer_Tick;

            InternetConnectionAvailability = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            _checkNetworkAvailability =new DispatcherTimer {Interval = TimeSpan.FromSeconds(5)};
            _checkNetworkAvailability.Tick += _checkNetworkAvailability_Tick;
            _checkNetworkAvailability.Start();


            MePlayer.Player.MediaOpened += Player_MediaOpened;
            MePlayer.Player.MediaEnded += Player_MediaEnded;
            AccountSignOut += MainViewModel_AccountSignOut;
        }

        private void _checkNetworkAvailability_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                InternetConnectionAvailability = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            });
        }

        private void MainViewModel_AccountSignOut(object sender, EventArgs e)
        {
            if(MePlayer.IsPlaying)
                MePlayer.StopAudio();

            MePlayer.VkAudio = null;

            Settings.SaveLoginPassword = false;
        }

        private  bool LoginToAccount()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) return false;
            try
            {
                var autorizeAuthParams = new ApiAuthParams
                {
                    Login = Settings.Login,
                    Password = Settings.Password,
                    ApplicationId = Appid,
                    Settings = VkNet.Enums.Filters.Settings.All
                };

                 VkApi.Authorize(autorizeAuthParams);
                OnAuthorizationSuccess();

                return true;
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

                 DialogCoordinator.Instance.ShowMessageAsync(this, "Îøèáêà", ex.Message, MessageDialogStyle.Affirmative, materialSettings);
            }
            return false;
        }

        private RelayCommand _timeSliderLeftButtonDown;
        public RelayCommand TimeSliderLeftButtonDown
        {
            get
            {
                return _timeSliderLeftButtonDown
                    ?? (_timeSliderLeftButtonDown = new RelayCommand(
                    () =>
                    {
                        MePlayer.UserIsDraggingSlider = true;
                        MePlayer.PauseAudio();
                        
                    }));
            }
        }

        private RelayCommand _timeSliderLeftButtonUp;
        public RelayCommand TimeSliderLeftButtonUp
        {
            get
            {
                return _timeSliderLeftButtonUp
                    ?? (_timeSliderLeftButtonUp = new RelayCommand(
                    () =>
                    {
                        MePlayer.PlayAudio();
                        MePlayer.UserIsDraggingSlider = false;
                        MePlayer.Player.Position =
                        TimeSpan.FromSeconds(((MainWindow)Application.Current.MainWindow).TimeSlider.Value);
                    }));
            }
        }

        private RelayCommand _timeSliderValueChanged;
        public RelayCommand TimeSliderValueChanged
        {
            get
            {
                return _timeSliderValueChanged
                    ?? (_timeSliderValueChanged = new RelayCommand(
                    () =>
                    {
                        if (MePlayer.UserIsDraggingSlider)
                        {
                            ((MainWindow) Application.Current.MainWindow).PlaybackTime.Text =
                                TimeSpan.FromSeconds(((MainWindow) Application.Current.MainWindow).TimeSlider.Value)
                                    .ToString(@"mm\:ss");
                        }
                    }));
            }
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            _timer.Stop();

            RaisePropertyChanged(() => MePlayer.Player.HasAudio);

            if (RepeatAudio)
            {
                MePlayer.StopAudio();
                MePlayer.PlayAudio();
                Player_MediaOpened(this,EventArgs.Empty);
            }
            else
            {
                MePlayer.NextAudio(Shuffle);
            }
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            _timer.Start();

            ((MainWindow)Application.Current.MainWindow).TimeSlider.Maximum =
                MePlayer.Player.NaturalDuration.TimeSpan.TotalSeconds;

            ((MainWindow) Application.Current.MainWindow).TotalPlayTime.Text =
                MePlayer.Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (MePlayer.Player.Source != null && MePlayer.Player.NaturalDuration.HasTimeSpan && !MePlayer.UserIsDraggingSlider)
            {
                ((MainWindow)Application.Current.MainWindow).TimeSlider.Value =
                    MePlayer.Player.Position.TotalSeconds;

                ((MainWindow)Application.Current.MainWindow).PlaybackTime.Text= MePlayer.Player.Position.ToString(@"mm\:ss");
            }
        }

        private RelayCommand _previewAudioTrackCommand;
        public RelayCommand PreviewAudioTrack
        {
            get
            {
                return  _previewAudioTrackCommand
                    ?? ( _previewAudioTrackCommand = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            MePlayer.PreviewAudio(Shuffle);
                            MePlayer.PlayAudio();
                        }
                        catch (NullReferenceException) {}
                       
                    }));
            }
        }

        private RelayCommand _nextAudioTrackCommand;
        public RelayCommand NextAudioTrack
        {
            get
            {
                return _nextAudioTrackCommand
                    ?? (_nextAudioTrackCommand = new RelayCommand(
                    () =>
                    {
                        try
                        {
                           MePlayer.NextAudio(Shuffle);
                            MePlayer.PlayAudio();
                        }
                        catch (NullReferenceException){}

                    }));
            }
        }

        public event EventHandler AuthorizationSuccess;
        public virtual void OnAuthorizationSuccess()
        {
            AuthorizationSuccess?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler AccountSignOut;

        public virtual void OnAccountSignOut()
        {
            AccountSignOut?.Invoke(this, EventArgs.Empty);
        }

        private RelayCommand _windowClosingCommand;
        public RelayCommand WindowClosing
        {
            get
            {
                return  _windowClosingCommand
                    ?? ( _windowClosingCommand = new RelayCommand(
                    () =>
                    {
                        using (StreamWriter sw = new StreamWriter("settings.data"))
                        {
                            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                            string jsonObject = JsonConvert.SerializeObject(Settings,Formatting.Indented, settings);
                            sw.Write(jsonObject);
                        }
                    }));
            }
        }

        private RelayCommand _windowLoadedCommand;
        public RelayCommand WindowLoaded
        {
            get
            {
                return _windowLoadedCommand
                    ?? (_windowLoadedCommand = new RelayCommand(
                    () =>
                    {
                        if (Settings.SaveLoginPassword && LoginToAccount())
                            AuthorizationPanel = new UserProfiler();
                        else
                            AuthorizationPanel = new AuthorizationControl();
                    }));
            }
        }
    }
}