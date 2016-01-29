using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using VkNet.Model;
using VkNet.Model.Attachments;
using WPFMusicPlayer.Enums;
using WPFMusicPlayer.ViewModel;

namespace WPFMusicPlayer.Classes
{
    public class AudioFile :ViewModelBase
    {

        public const string PlayerPropertyName = "Player";
        private MediaPlayer _player;
        public MediaPlayer Player
        {
            get
            {
                return _player;
            }

            set
            {
                if (_player == value)
                {
                    return;
                }

                _player = value;
                RaisePropertyChanged(PlayerPropertyName);
            }
        }

        public const string VkAudioPropertyName = "VkAudio";
        private Audio _vkAudio = null;
        public Audio VkAudio
        {
            get
            {
                return _vkAudio;
            }

            set
            {
                if (_vkAudio == value)
                {
                    return;
                }

                _vkAudio = value;
                Player.Open(VkAudio.Url);
                RaisePropertyChanged(VkAudioPropertyName);
            }
        }

        public const string UsedListPropertyName = "UsedList";
        private UserAudioList _usedList = null;
        public UserAudioList UsedList
        {
            get
            {
                return _usedList;
            }

            set
            {
                if (_usedList == value)
                {
                    return;
                }

                _usedList = value;
                RaisePropertyChanged(UsedListPropertyName);
            }
        }


        public const string IsPlayingPropertyName = "IsPlaying";
        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }

            set
            {
                if (_isPlaying == value)
                {
                    return;
                }

                _isPlaying = value;
                RaisePropertyChanged(IsPlayingPropertyName);
            }
        }


        public bool UserIsDraggingSlider { get; set; }

        public AudioFile():base()
        {
            Player = new MediaPlayer();
            IsPlaying = false;
        }


        public void PlayAudio()
        {
            if (_vkAudio == null)
                throw new NullReferenceException();

            Player.Play();
            IsPlaying = true;
        }

        public void PauseAudio()
        {
            if (Player.CanPause)
            {
                Player.Pause();
                IsPlaying = false;
            }
        }

        public void StopAudio()
        {
            if (IsPlaying)
            {
                Player.Stop();
                IsPlaying = false;
            }
        }

        public void NextAudio(bool isRandom=false)
        {
            var audios = ((UserAudioListViewModel) UsedList.DataContext).Audios;
            if (audios.Count == 0)
                return;

            UserAudioListViewModel.ChangeListPlayButtonVisibility(VkAudio, Visibility.Collapsed);

            if (isRandom)
            {
                Random rand = new Random();
                int index = rand.Next(0, audios.Count - 1);

                VkAudio = audios[index];
                UsedList.AudiosList.SelectedItem = VkAudio;
            }
            else
            {
                int index = audios.IndexOf(VkAudio);
                if (++index != audios.Count)
                {
                    VkAudio = audios[index];
                    UsedList.AudiosList.SelectedItem = VkAudio;
                }
            }

        }

        public void PreviewAudio(bool isRandom=false)
        {
            var audios = ((UserAudioListViewModel)UsedList.DataContext).Audios;
            if (audios.Count == 0)
                return;
            
            UserAudioListViewModel.ChangeListPlayButtonVisibility(VkAudio, Visibility.Collapsed);
            if (isRandom)
            {
                Random rand=new Random();
                int index = rand.Next(0, audios.Count - 1);

                VkAudio = audios[index];
                UsedList.AudiosList.SelectedItem = VkAudio;
            }
            else
            {
                int index = audios.IndexOf(VkAudio);
                if (--index >= 0)
                {
                    VkAudio = audios[index];
                    UsedList.AudiosList.SelectedItem = VkAudio;
                }
            }

        }

        private RelayCommand<bool> _playPauseCommand;
        public RelayCommand<bool> PlayPause
        {
            get
            {
                return _playPauseCommand
                    ?? (_playPauseCommand = new RelayCommand<bool>(
                    (isCheked) =>
                    {
                        if (isCheked)
                        {
                            PlayAudio();
                        }
                        else
                        {
                            PauseAudio();
                        }
                    }));
            }
        }
    }
}