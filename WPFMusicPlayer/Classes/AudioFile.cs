using System;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using VkNet.Model.Attachments;

namespace WPFMusicPlayer.Classes
{
    public class AudioFile :ViewModelBase
    {
        public MediaPlayer Player { get; set; }


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

        public bool isPlaying { get; private set; }

        public AudioFile():base()
        {
            Player = new MediaPlayer();
            isPlaying = false;
        }

        public void PlayAudio()
        {
            if (_vkAudio == null)
                throw new NullReferenceException();

            Player.Play();
            isPlaying = true;
        }

        public void PauseAudio()
        {
            if (isPlaying && Player.CanPause)
            {
                Player.Pause();
                isPlaying = false;
            }
        }

        public void StopAudio()
        {
            if (isPlaying)
            {
                Player.Stop();
                isPlaying = false;
            }
        }
    }
}