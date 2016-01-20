using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
        private VkApi _vkApi;

        public const string AudiosPropertyName = "Audios";
        private ObservableCollection<Audio> _audios = new ObservableCollection<Audio>();
        public ObservableCollection<Audio> Audios
        {
            get
            {
                return _audios;
            }

            set
            {
                if (_audios == value)
                {
                    return;
                }

                _audios = value;
                RaisePropertyChanged(AudiosPropertyName);
            }
        }



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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _autorizeAuthParams=new ApiAuthParams();

            _vkApi=new VkApi();
            
            
            _autorizeAuthParams.Login = "shyrovec@rambler.ru";
            _autorizeAuthParams.Password = "Stelmuhov";
            _autorizeAuthParams.ApplicationId = APPID;
            _autorizeAuthParams.Settings = Settings.All;


            _vkApi.Authorize(_autorizeAuthParams);

            Audios=new ObservableCollection<Audio>(_vkApi.Audio.Get((ulong)_vkApi.UserId.Value));
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
          where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private RelayCommand _listViewItemChanged;
        public RelayCommand ListViewItemChanged
        {
            get
            {
                return _listViewItemChanged
                    ?? (_listViewItemChanged = new RelayCommand(
                    () =>
                    {


                        MakeCanvasPictureVisible(Picture.Play, SelectedAudio.VkAudio);
                        MakeCanvasPictureVisible(Picture.Pause, ((MainWindow)Application.Current.MainWindow).AudiosList.SelectedItem);
                        SelectedAudio.VkAudio=((MainWindow)Application.Current.MainWindow).AudiosList.SelectedItem as Audio;
                        SelectedAudio.PlayAudio();
                    }));
            }
        }

        private RelayCommand _listViewItemClick;
        public RelayCommand ListViewItemClick
        {
            get
            {
                return _listViewItemClick
                    ?? (_listViewItemClick = new RelayCommand(
                    () =>
                    {
                        if (SelectedAudio.VkAudio !=
                            ((MainWindow) Application.Current.MainWindow).AudiosList.SelectedItem)
                            return;

                        if (SelectedAudio.isPlaying)
                        {
                            MakeCanvasPictureVisible(Picture.Play, SelectedAudio.VkAudio);
                            SelectedAudio.PauseAudio();
                        }
                        else
                        {
                            MakeCanvasPictureVisible(Picture.Pause, SelectedAudio.VkAudio);
                            SelectedAudio.PlayAudio();
                        }
                    }));
            }
        }

        private void MakeCanvasPictureVisible(Picture picture,object listItem)
        {
            ListBoxItem listBoxItem =
                            (ListBoxItem)
                                (((MainWindow)Application.Current.MainWindow).AudiosList.ItemContainerGenerator
                                    .ContainerFromItem(
                                        listItem));
            if(listBoxItem==null) return;
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;

            Canvas itemCanvas = (Canvas)dataTemplate.FindName("AudioListItemCanvas", contentPresenter);

            Path playFigure = (Path)itemCanvas.FindName("PlayPath");
            Path pauseFigure = (Path)itemCanvas.FindName("PausePath");

            switch (picture)
            {
                case Picture.Pause:
                {
                    pauseFigure.Visibility = Visibility.Visible;
                    playFigure.Visibility=Visibility.Hidden;

                    break;
                }

                case Picture.Play:
                {
                    playFigure.Visibility = Visibility.Visible;
                    pauseFigure.Visibility = Visibility.Hidden;

                    break;
                }
            }
        }
    }
}