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
using VkNet.Enums;
using VkNet.Model.Attachments;
using WPFMusicPlayer.Classes;
using WPFMusicPlayer.Views;

namespace WPFMusicPlayer.ViewModel
{
    public class AudioSearchViewModel:AudioListViewModel
    {

        public const string TextToSearcPropertyName = "TextToSearc";
        private string _textToSearch;
        public string TextToSearch
        {
            get
            {
                return _textToSearch;
            }

            set
            {
                if (_textToSearch == value)
                {
                    return;
                }

                _textToSearch = value;
                RaisePropertyChanged(TextToSearcPropertyName);
            }
        }

        public const string IsActivePropertyName = "IsActive";
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                if (_isActive == value)
                {
                    return;
                }

                _isActive = value;
                RaisePropertyChanged(IsActivePropertyName);
            }
        }

        public AudioSearchViewModel()
        {
            MainVm.MePlayer.VkAudioChanged += SelectedAudio_VkAudioChanged;
            MainVm.AuthorizationSuccess += MainVm_AuthorizationSuccess;
            MainVm.AccountSignOut += MainVm_AccountSignOut;
        }

        private void MainVm_AccountSignOut(object sender, EventArgs e)
        {
            IsActive = false;
        }

        private void MainVm_AuthorizationSuccess(object sender, EventArgs e)
        {
            IsActive = true;
        }

        public override void ChangeListPlayButtonVisibility(object listItem, Visibility visibility)
        {
            ListBoxItem listBoxItem =
                (ListBoxItem)
                    (((AudioSearch)((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content).AudiosList.ItemContainerGenerator
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
                    (((AudioSearch)((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content).AudiosList.ItemContainerGenerator
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

        private void PlayNewAudioFromList()
        {
            MainVm.MePlayer.UsedList = ((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content as UserControl;

            SwitchListPlayButtonVisibility(((AudioSearch)((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content).AudiosList.SelectedItem);

            MainVm.MePlayer.VkAudio = ((AudioSearch)((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content).AudiosList.SelectedItem as Audio;

            if (MainVm.MePlayer.VkAudio != null)
                MainVm.MePlayer.PlayAudio();
        }

        protected override void UpdateAudioList()
        {}

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
                            var userAudios = new List<Audio>(MainVm.VkApi.Audio.Get(MainVm.VkApi.UserId.Value));

                            if (userAudios.FirstOrDefault(a => a.Title == audio.Title && a.Artist == audio.Artist) == null && audio.OwnerId != null)
                                MainVm.VkApi.Audio.Add(audioId, audio.OwnerId.Value);
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
                                    AnimateShow = true,
                                    AnimateHide = true,
                                    ColorScheme = MetroDialogColorScheme.Accented
                                };

                                await DialogCoordinator.Instance.ShowMessageAsync(this, "Ошибка", "Данная аудиозапись уже присуцтвует в вашем списке", MessageDialogStyle.Affirmative, materialSettings);
                            }
                        }
                    }));
            }
        }

        private void SelectedAudio_VkAudioChanged(object sender, EventArgs e)
        {
            if (MainVm.MePlayer.UsedList.GetType() == typeof(AudioSearch))
            {
                ((AudioSearch)MainVm.MePlayer.UsedList).AudiosList.SelectedItem = MainVm.MePlayer.VkAudio;
            }
            else if (((AudioSearch)((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content).AudiosList.SelectedItem != null)
            {
                ((AudioSearch)((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content).AudiosList.SelectedItem = null;
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
                        MainVm.MePlayer.UsedList = ((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content as UserControl;

                        if (MainVm.MePlayer.VkAudio == null)
                            return;

                        if (MainVm.MePlayer.VkAudio !=
    ((AudioSearch)((MainWindow)Application.Current.MainWindow).AudioSearchListItem.Content)
        .AudiosList.SelectedItem)
                        {
                            PlayNewAudioFromList();
                            RaisePropertyChanged(() => MainVm.MePlayer.IsPlaying);
                            return;
                        }

                        if (MainVm.MePlayer.IsPlaying)
                            MainVm.MePlayer.PauseAudio();
                        else
                            MainVm.MePlayer.PlayAudio();
                    }));
            }
        }

        private RelayCommand _listViewItemChanged;
        public RelayCommand ListViewItemChanged => _listViewItemChanged
                                                   ?? (_listViewItemChanged = new RelayCommand(
                                                       PlayNewAudioFromList));


        private RelayCommand _searchCommand;
        public RelayCommand SearchAudio
        {
            get
            {
                return _searchCommand
                    ?? (_searchCommand = new RelayCommand(
                    () =>
                    {
                        
                        Task loadAudiosTask=new Task(SearchAudiosByText);
                        loadAudiosTask.Start();
                    }));
            }
        }

        private void SearchAudiosByText()
        {
            MainVm.ShowProgressBar = true;

            long totalCount;
            if (MainVm.VkApi.UserId != null)
                Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.Search(TextToSearch, out totalCount, true, AudioSort.Popularity, false, 300, 0));

            MainVm.ShowProgressBar = false;

        }
    }
}