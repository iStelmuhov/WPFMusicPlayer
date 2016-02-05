using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using VkNet.Model.Attachments;
using WPFMusicPlayer.Classes;

namespace WPFMusicPlayer.ViewModel
{
    public class RecommendationsAudioListViewModel: AudioListViewModel
    {

        public RecommendationsAudioListViewModel()
        {
            Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.GetRecommendations((ulong)MainVm.VkApi.UserId.Value));
            MainVm.SelectedAudio.VkAudioChanged += SelectedAudio_VkAudioChanged;
        }

        public RecommendationsAudioListViewModel(MainViewModel mvm)
        {
            MainVm = mvm;

            Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.Get((ulong)MainVm.VkApi.UserId.Value));
        }

        private void PlayNewAudioFromList()
        {
            MainVm.SelectedAudio.UsedList = ((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content as UserControl;

            SwitchListPlayButtonVisibility(((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem);
            //ChangeListPlayButtonVisibility(MainVm.SelectedAudio.VkAudio, Visibility.Collapsed);
            //ChangeListPlayButtonVisibility(((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem, Visibility.Visible);

            MainVm.SelectedAudio.VkAudio = ((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem as Audio;
            MainVm.SelectedAudio.PlayAudio();
        }

        private RelayCommand _listViewItemChanged;
        public RelayCommand ListViewItemChanged => _listViewItemChanged
                                                   ?? (_listViewItemChanged = new RelayCommand(
                                                       PlayNewAudioFromList));

        private RelayCommand _listViewItemClick;
        public RelayCommand ListViewItemClick
        {
            get
            {
                return _listViewItemClick
                    ?? (_listViewItemClick = new RelayCommand(
                    () =>
                    {
                        MainVm.SelectedAudio.UsedList = ((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content as UserControl;

                        if ( MainVm.SelectedAudio.VkAudio == null)
                            return;

                        if (MainVm.SelectedAudio.VkAudio !=
    ((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content)
        .AudiosList.SelectedItem)
                        {
                            PlayNewAudioFromList();
                            RaisePropertyChanged(()=>MainVm.SelectedAudio.IsPlaying);
                            return;
                        }

                        if (MainVm.SelectedAudio.IsPlaying)
                            MainVm.SelectedAudio.PauseAudio();
                        else
                            MainVm.SelectedAudio.PlayAudio();
                    }));
            }
        }

        private void SelectedAudio_VkAudioChanged(object sender, EventArgs e)
        {
            if (MainVm.SelectedAudio.UsedList.GetType() == typeof(RecommendationsAudioList))
            {
                ((RecommendationsAudioList)MainVm.SelectedAudio.UsedList).AudiosList.SelectedItem = MainVm.SelectedAudio.VkAudio;
            }
            else if(((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem !=null)
            {
                ((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem = null;
            }
        }

        private RelayCommand<long> _removeAudioCommand;
        public RelayCommand<long> RemoveAudio
        {
            get
            {
                return _removeAudioCommand
                    ?? (_removeAudioCommand = new RelayCommand<long>(
                    (audioId) =>
                    {
                        var audio = Audios.First(a => a.Id == audioId);

                        if (audio == MainVm.SelectedAudio.VkAudio)
                        {
                            if (!MainVm.SelectedAudio.NextAudio(false) && !MainVm.SelectedAudio.PreviewAudio(false))
                            {
                                MainVm.SelectedAudio.VkAudio = new Audio();
                            }
                        }


                        MainVm.VkApi.Audio.Delete((ulong)audioId, audio.OwnerId.Value);
                        Audios.Remove(audio);

                        RaisePropertyChanged(AudiosPropertyName);
                    }));
            }
        }



        public override void ChangeListPlayButtonVisibility(object listItem, Visibility visibility)
        {
            ListBoxItem listBoxItem =
                            (ListBoxItem)
                                (((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.ItemContainerGenerator
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
                                (((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.ItemContainerGenerator
                                    .ContainerFromItem(
                                        listItem));
            if (listBoxItem == null) return;
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;

            if(VisibleItem!=null)
                VisibleItem.Visibility=Visibility.Collapsed;

            VisibleItem = (ToggleButton)dataTemplate.FindName("PausePlayButton", contentPresenter);
            VisibleItem.Visibility = Visibility.Visible;
        }
    }
}