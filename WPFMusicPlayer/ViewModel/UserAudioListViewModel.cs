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
using Page = System.Windows.Controls.Page;

namespace WPFMusicPlayer.ViewModel
{
    public class UserAudioListViewModel:ViewModelBase
    {
        private MainViewModel _mainVm ;

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

        public UserAudioListViewModel()
        {
            _mainVm = ((MainViewModel) Application.Current.MainWindow.DataContext);
            Audios = new ObservableCollection<Audio>(_mainVm.VkApi.Audio.Get((ulong)_mainVm.VkApi.UserId.Value));

            
        }

        public UserAudioListViewModel(MainViewModel mvm)
        {
            _mainVm = mvm;

            Audios = new ObservableCollection<Audio>(_mainVm.VkApi.Audio.Get((ulong)_mainVm.VkApi.UserId.Value));
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


                        MakeCanvasPictureVisible(Picture.Play,_mainVm.SelectedAudio.VkAudio);

                        MakeCanvasPictureVisible(Picture.Pause, ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem);
                        _mainVm.SelectedAudio.VkAudio = ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem as Audio;
                        _mainVm.SelectedAudio.PlayAudio();
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
                        _mainVm.SelectedAudio.UsedList = ((MainWindow)Application.Current.MainWindow).UserListItem.Content as UserAudioList;

                        if (_mainVm.SelectedAudio.VkAudio !=
                            ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem)
                            return;
                        ///////!!!!!!!!!!!!!!!! UP
                        
                        if (_mainVm.SelectedAudio.IsPlaying)
                        {
                            MakeCanvasPictureVisible(Picture.Play, _mainVm.SelectedAudio.VkAudio);
                            _mainVm.SelectedAudio.PauseAudio();
                        }
                        else
                        {
                            MakeCanvasPictureVisible(Picture.Pause, _mainVm.SelectedAudio.VkAudio);
                            _mainVm.SelectedAudio.PlayAudio();
                        }
                    }));
            }
        }

        private void MakeCanvasPictureVisible(Picture picture, object listItem)
        {
            ListBoxItem listBoxItem =
                            (ListBoxItem)
                                (((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.ItemContainerGenerator
                                    .ContainerFromItem(
                                        listItem));
            if (listBoxItem == null) return;
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
                        playFigure.Visibility = Visibility.Hidden;

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