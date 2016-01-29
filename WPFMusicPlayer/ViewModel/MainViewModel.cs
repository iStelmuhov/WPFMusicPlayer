using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.Attachments;
using WPFMusicPlayer.Classes;
using WPFMusicPlayer.Enums;

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
    public class MainViewModel : ViewModelBase
    {
        public const ulong APPID = 5233775;

        private ApiAuthParams _autorizeAuthParams;
        public VkApi VkApi;

        private DispatcherTimer _timer;

        public const string SelectedAudioPropertyName = "SelectedAudio";
        private AudioFile _selectedAudioFile = new AudioFile();
        public AudioFile SelectedAudio
        {
            get
            {
                return _selectedAudioFile;
            }

            set
            {
                if (_selectedAudioFile == value)
                {
                    return;
                }

                _selectedAudioFile = value;
                RaisePropertyChanged(SelectedAudioPropertyName);
            }
        }


        public const string RepeatAudioPropertyName = "RepeatAudio";
        private bool _repeatAudio = false;
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
                RaisePropertyChanged(RepeatAudioPropertyName);
            }
        }


        public const string ShufflePropertyName = "Shuffle";
        private bool _shuffle = false;
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
                RaisePropertyChanged(ShufflePropertyName);
            }
        }

        public MainViewModel()
        {
            _autorizeAuthParams=new ApiAuthParams();

            VkApi=new VkApi();

            _autorizeAuthParams.Login = "shyrovec@rambler.ru";
            _autorizeAuthParams.Password = "Stelmuhov";
            _autorizeAuthParams.ApplicationId = APPID;
            _autorizeAuthParams.Settings = Settings.All;
            
             VkApi.Authorize(_autorizeAuthParams);

            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timer.Tick += _timer_Tick;
           

            SelectedAudio.Player.MediaOpened += Player_MediaOpened;
            SelectedAudio.Player.MediaEnded += Player_MediaEnded;
        }


        private RelayCommand<bool> _myCommand;
        public RelayCommand<bool> MyCommand
        {
            get
            {
                return _myCommand
                    ?? (_myCommand = new RelayCommand<bool>(
                    (isDark) =>
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
                        SelectedAudio.UserIsDraggingSlider = true;
                        SelectedAudio.PauseAudio();
                        
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
                        SelectedAudio.PlayAudio();
                        SelectedAudio.UserIsDraggingSlider = false;
                        SelectedAudio.Player.Position =
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
                        if (SelectedAudio.UserIsDraggingSlider)
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

            RaisePropertyChanged(() => SelectedAudio.Player.HasAudio);

            if (RepeatAudio)
            {
                SelectedAudio.StopAudio();
                SelectedAudio.PlayAudio();
                Player_MediaOpened(this,EventArgs.Empty);
            }
            else
            {
                SelectedAudio.NextAudio(Shuffle);
            }
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            _timer.Start();

            ((MainWindow)Application.Current.MainWindow).TimeSlider.Maximum =
                SelectedAudio.Player.NaturalDuration.TimeSpan.TotalSeconds;

            ((MainWindow) Application.Current.MainWindow).TotalPlayTime.Text =
                SelectedAudio.Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (SelectedAudio.Player.Source != null && SelectedAudio.Player.NaturalDuration.HasTimeSpan && !SelectedAudio.UserIsDraggingSlider)
            {
                ((MainWindow)Application.Current.MainWindow).TimeSlider.Value =
                    SelectedAudio.Player.Position.TotalSeconds;

                ((MainWindow)Application.Current.MainWindow).PlaybackTime.Text= SelectedAudio.Player.Position.ToString(@"mm\:ss");
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
                            SelectedAudio.PreviewAudio(Shuffle);
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
                           SelectedAudio.NextAudio(Shuffle);
                        }
                        catch (NullReferenceException){}

                    }));
            }
        }
    }
}