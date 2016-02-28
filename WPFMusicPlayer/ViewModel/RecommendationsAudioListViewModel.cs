using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using VkNet.Model.Attachments;
using WPFMusicPlayer.Classes;

namespace WPFMusicPlayer.ViewModel
{
    public class RecommendationsAudioListViewModel: AudioListViewModel
    {

        public RecommendationsAudioListViewModel()
        {          
            MainVm.MePlayer.VkAudioChanged += SelectedAudio_VkAudioChanged;
        }

        private void PlayNewAudioFromList()
        {
            MainVm.MePlayer.UsedList = ((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content as UserControl;

            SwitchListPlayButtonVisibility(((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem);

            MainVm.MePlayer.VkAudio = ((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem as Audio;

            if(MainVm.MePlayer.VkAudio!=null)
                MainVm.MePlayer.PlayAudio();
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
                        MainVm.MePlayer.UsedList = ((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content as UserControl;

                        if ( MainVm.MePlayer.VkAudio == null)
                            return;

                        if (MainVm.MePlayer.VkAudio !=
    ((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content)
        .AudiosList.SelectedItem)
                        {
                            PlayNewAudioFromList();
                            RaisePropertyChanged(()=>MainVm.MePlayer.IsPlaying);
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
            if (MainVm.MePlayer.UsedList.GetType() == typeof(RecommendationsAudioList))
            {
                ((RecommendationsAudioList)MainVm.MePlayer.UsedList).AudiosList.SelectedItem = MainVm.MePlayer.VkAudio;
            }
            else if(((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem !=null)
            {
                ((RecommendationsAudioList)((MainWindow)Application.Current.MainWindow).RecommendationsListItem.Content).AudiosList.SelectedItem = null;
            }
        }

        private RelayCommand<long> _addAudioCommand;
        public RelayCommand<long> AddAudio
        {
            get
            {
                return _addAudioCommand
                    ?? (_addAudioCommand = new RelayCommand<long>(async audioId =>
                    {
                        var audio = Audios.First(a => a.Id == audioId);
                        if (MainVm.VkApi.UserId != null)
                        {
                            var userAudios = new List<Audio>(MainVm.VkApi.Audio.Get((ulong)MainVm.VkApi.UserId.Value));

                            if (userAudios.FirstOrDefault(a => a.Title == audio.Title && a.Artist == audio.Artist) == null && audio.OwnerId!=null)
                                MainVm.VkApi.Audio.Add((ulong) audioId, audio.OwnerId.Value);
                            else
                            {
                                var materialSettings = new MetroDialogSettings
                                {
                                    CustomResourceDictionary =
                                        new ResourceDictionary()
                                        {
                                            Source =
                                                new Uri(
                                                    "pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
                                        },
                                    SuppressDefaultResources = true,
                                    AnimateShow=true,
                                    AnimateHide = true,
                                    ColorScheme= MetroDialogColorScheme.Accented
                                };

                                await DialogCoordinator.Instance.ShowMessageAsync(this,"Ошибка", "Данная аудиозапись уже присуцтвует в вашем списке", MessageDialogStyle.Affirmative, materialSettings);
                            }
                        }
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

        public override void UpdateAudioList()
        {
            if (MainVm.VkApi.UserId != null)
                Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.GetRecommendations((ulong)MainVm.VkApi.UserId.Value));
        }
    }
}