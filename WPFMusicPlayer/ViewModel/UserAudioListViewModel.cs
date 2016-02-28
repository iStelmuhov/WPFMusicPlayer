using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight.CommandWpf;
using VkNet.Model.Attachments;
using WPFMusicPlayer.Classes;

namespace WPFMusicPlayer.ViewModel
{
    public class UserAudioListViewModel: AudioListViewModel
    {
        public UserAudioListViewModel()
        {
            
            MainVm.MePlayer.VkAudioChanged += SelectedAudio_VkAudioChanged;
        }

        private void PlayNewAudioFromList()
        {
            MainVm.MePlayer.UsedList = ((MainWindow)Application.Current.MainWindow).UserListItem.Content as UserAudioList;

            SwitchListPlayButtonVisibility(((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem);
            MainVm.MePlayer.VkAudio = ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem as Audio;

            if(MainVm.MePlayer.VkAudio!=null)
                MainVm.MePlayer.PlayAudio();
        }

        private RelayCommand _listViewItemChangedCommand;

        public RelayCommand ListViewItemChanged
        {
            get
            {
                return _listViewItemChangedCommand
                    ?? (_listViewItemChangedCommand = new RelayCommand(
                    () =>
                    {
                        PlayNewAudioFromList();

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
                        MainVm.MePlayer.UsedList = ((MainWindow)Application.Current.MainWindow).UserListItem.Content as UserControl;

                        if (MainVm.MePlayer.VkAudio == null)
                            return;

                        if (MainVm.MePlayer.VkAudio !=
                            ((UserAudioList) ((MainWindow) Application.Current.MainWindow).UserListItem.Content)
                                .AudiosList.SelectedItem)
                        {
                            PlayNewAudioFromList();
                            return;
                        }

                        if (MainVm.MePlayer.IsPlaying)
                            MainVm.MePlayer.PauseAudio();
                        else
                            MainVm.MePlayer.PlayAudio();
                    }));
            }
        }
  
        private void SelectedAudio_VkAudioChanged(object sender, EventArgs e)
        {
            if (MainVm.MePlayer.UsedList.GetType() == typeof (UserAudioList))
            {
                ((UserAudioList)MainVm.MePlayer.UsedList).AudiosList.SelectedItem = MainVm.MePlayer.VkAudio;
            }
            else if (((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem != null)
            {
                ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem = null;
            }
        }

        private RelayCommand<long> _removeAudioCommand;
        public RelayCommand<long> RemoveAudio
        {
            get
            {
                return _removeAudioCommand
                    ?? (_removeAudioCommand = new RelayCommand<long>(
                    audioId =>
                    {
                        var audio = Audios.First(a => a.Id == audioId);

                        if (audio == MainVm.MePlayer.VkAudio)
                        {
                            if (!MainVm.MePlayer.NextAudio() && !MainVm.MePlayer.PreviewAudio())
                            {
                                MainVm.MePlayer.VkAudio=new Audio();
                            }
                        }


                        if (audio.OwnerId != null) MainVm.VkApi.Audio.Delete((ulong)audioId, audio.OwnerId.Value);
                            Audios.Remove(audio);

                        // ReSharper disable once ExplicitCallerInfoArgument
                        RaisePropertyChanged(AudiosPropertyName);
                    }));
            }
        }

        private RelayCommand _updateAudioListCommand;
        public RelayCommand UpdateAudioListCommand => _updateAudioListCommand
                                                      ?? ( _updateAudioListCommand = new RelayCommand(
                                                          UpdateAudioList));

        public override void ChangeListPlayButtonVisibility(object listItem, Visibility visibility)
        {
            ListBoxItem listBoxItem =
                            (ListBoxItem)
                                (((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.ItemContainerGenerator
                                    .ContainerFromItem(
                                        listItem));
            if (listBoxItem == null) return;
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;

            ToggleButton itemToggleButton = (ToggleButton)dataTemplate.FindName("PausePlayButton", contentPresenter);
            itemToggleButton.Visibility = visibility;
        }

        public override void SwitchListPlayButtonVisibility(object listItem)
        { 
            ListBoxItem listBoxItem =
                            (ListBoxItem)
                                (((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.ItemContainerGenerator
                                    .ContainerFromItem(
                                        listItem));
            if (listBoxItem == null) return;
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;

            if (VisibleItem != null)
                VisibleItem.Visibility = Visibility.Collapsed;

            VisibleItem = (ToggleButton)dataTemplate.FindName("PausePlayButton", contentPresenter);
            VisibleItem.Visibility = Visibility.Visible;
        }

        public override void UpdateAudioList()
        {
            if (MainVm.VkApi.UserId != null)
                Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.Get((ulong)MainVm.VkApi.UserId.Value));
        }
    }
}