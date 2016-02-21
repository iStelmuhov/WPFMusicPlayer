using System;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using VkNet;
using VkNet.Enums.Filters;
using WPFMusicPlayer.Classes;

namespace WPFMusicPlayer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    /// 
    public class MainViewModel : ViewModelBase
    {
        public const ulong Appid = 5233775;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ApiAuthParams _autorizeAuthParams;
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

        public MainViewModel()
        {
            _autorizeAuthParams=new ApiAuthParams();


            



            VkApi =new VkApi();

            _autorizeAuthParams.Login = "shyrovec@rambler.ru";
            _autorizeAuthParams.Password = "Stelmuhov";
            _autorizeAuthParams.ApplicationId = Appid;
            _autorizeAuthParams.Settings = Settings.All;
            
             VkApi.Authorize(_autorizeAuthParams);

            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timer.Tick += _timer_Tick;
           

            MePlayer.Player.MediaOpened += Player_MediaOpened;
            MePlayer.Player.MediaEnded += Player_MediaEnded;
        }


        private RelayCommand<bool> _myCommand;
        public RelayCommand<bool> MyCommand
        {
            get
            {
                return _myCommand
                    ?? (_myCommand = new RelayCommand<bool>(isDark =>
                    {
                        int a = 2;

                        new PaletteHelper().SetLightDark(isDark);
                    }));
            }
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
    }
}