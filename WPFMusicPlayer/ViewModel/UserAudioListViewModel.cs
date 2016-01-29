using System;
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
        public MainViewModel MainVm { get; set; }

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
            MainVm = ((MainViewModel) Application.Current.MainWindow.DataContext);
            Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.Get((ulong)MainVm.VkApi.UserId.Value));

            
        }

        public UserAudioListViewModel(MainViewModel mvm)
        {
            MainVm = mvm;

            Audios = new ObservableCollection<Audio>(MainVm.VkApi.Audio.Get((ulong)MainVm.VkApi.UserId.Value));
        }

        private static childItem FindVisualChild<childItem>(DependencyObject obj)
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
                        MainVm.SelectedAudio.UsedList = ((MainWindow)Application.Current.MainWindow).UserListItem.Content as UserAudioList;

                        ChangeListPlayButtonVisibility(MainVm.SelectedAudio.VkAudio,Visibility.Collapsed);
                        ChangeListPlayButtonVisibility(((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem,Visibility.Visible);

                        MainVm.SelectedAudio.VkAudio = ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem as Audio;
                        MainVm.SelectedAudio.PlayAudio();
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
                        MainVm.SelectedAudio.UsedList = ((MainWindow)Application.Current.MainWindow).UserListItem.Content as UserAudioList;


                        if (MainVm.SelectedAudio.VkAudio !=
                            ((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.SelectedItem || MainVm.SelectedAudio.VkAudio==null)
                            return;
                        ///////!!!!!!!!!!!!!!!! UP
                        
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


        public static void ChangeListPlayButtonVisibility(object listItem, Visibility visibility)
        {
            ListBoxItem listBoxItem =
                            (ListBoxItem)
                                (((UserAudioList)((MainWindow)Application.Current.MainWindow).UserListItem.Content).AudiosList.ItemContainerGenerator
                                    .ContainerFromItem(
                                        listItem));
            if (listBoxItem == null) return;
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;

            ToggleButton itemToggleButton = (ToggleButton) dataTemplate.FindName("PausePlayButton", contentPresenter);
            itemToggleButton.Visibility = visibility;
        }

    }
}