using System;
using System.Linq;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using VkNet.Model;
using VkNet.Model.Attachments;
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
        private Audio _vkAudio = new Audio();
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

        public bool IsPlaying { get; private set; }
        public bool UserIsDraggingSlider { get; set; }



        public AudioFile():base()
        {
            Player = new MediaPlayer();
            IsPlaying = false;

            Player.MediaEnded += Player_MediaEnded;
        }



        private void Player_MediaEnded(object sender, EventArgs e)
        {
            NextAudio();
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
            if (IsPlaying && Player.CanPause)
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

        public void NextAudio()
        {
            var Audios = ((UserAudioListViewModel) UsedList.DataContext).Audios;
            int index = Audios.IndexOf(VkAudio);

            if(index != Audios.Count)
                VkAudio = Audios[++index];
        }
    }
}