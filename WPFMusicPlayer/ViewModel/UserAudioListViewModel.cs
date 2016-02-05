﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.Attachments;
using WPFMusicPlayer.Classes;
using WPFMusicPlayer.Enums;
using Page = System.Windows.Controls.Page;

namespace WPFMusicPlayer.ViewModel
{
    public class UserAudioListViewModel: AudioListViewModel
    {
        private void PlayNewAudioFromList()
        {
            MainVm.SelectedAudio.UsedList = ((MainWindow)Application.Current.MainWindow).UserListItem.Content as UserAudioList;

            SwitchListPlayButtonVisibility(((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem);
//            ChangeListPlayButtonVisibility(MainVm.SelectedAudio.VkAudio, Visibility.Collapsed);
//            ChangeListPlayButtonVisibility(((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem, Visibility.Visible);

            MainVm.SelectedAudio.VkAudio = ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem as Audio;
            MainVm.SelectedAudio.PlayAudio();
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
                        MainVm.SelectedAudio.UsedList = ((MainWindow)Application.Current.MainWindow).UserListItem.Content as UserControl;

                        if (MainVm.SelectedAudio.VkAudio == null)
                            return;

                        if (MainVm.SelectedAudio.VkAudio !=
                            ((UserAudioList) ((MainWindow) Application.Current.MainWindow).UserListItem.Content)
                                .AudiosList.SelectedItem)
                        {
                            PlayNewAudioFromList();
                            return;
                        }

                        if (MainVm.SelectedAudio.IsPlaying)
                        {
                            MainVm.SelectedAudio.PauseAudio();
                        }
                        else
                        {
                            MainVm.SelectedAudio.PlayAudio();
                        }
                    }));
            }
        }


        public UserAudioListViewModel()
        {            
            Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.Get((ulong)MainVm.VkApi.UserId.Value));
            MainVm.SelectedAudio.VkAudioChanged += SelectedAudio_VkAudioChanged;          
        }

        private void SelectedAudio_VkAudioChanged(object sender, EventArgs e)
        {
            if (MainVm.SelectedAudio.UsedList.GetType() == typeof (UserAudioList))
            {
                ((UserAudioList)MainVm.SelectedAudio.UsedList).AudiosList.SelectedItem = MainVm.SelectedAudio.VkAudio;
            }
            else if (((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem != null)
            {
                ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem = null;
            }
        }

        public UserAudioListViewModel(MainViewModel mvm)
        {
            MainVm = mvm;

            Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.Get((ulong)MainVm.VkApi.UserId.Value));
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
                                MainVm.SelectedAudio.VkAudio=new Audio();
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
    }
}