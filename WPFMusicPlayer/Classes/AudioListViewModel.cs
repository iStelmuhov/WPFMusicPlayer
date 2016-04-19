using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using VkNet.Model.Attachments;
using WPFMusicPlayer.ViewModel;

namespace WPFMusicPlayer.Classes
{
    public abstract class AudioListViewModel:ViewModelBase
    {
        public MainViewModel MainVm { get; }

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

        public const string DownloadProgressValuePropertyName = "DownloadProgressValue";
        private int _downloadProgress = 0;
        public int DownloadProgressValue
        {
            get
            {
                return _downloadProgress;
            }

            set
            {
                if (_downloadProgress == value)
                {
                    return;
                }

                _downloadProgress = value;
                RaisePropertyChanged(DownloadProgressValuePropertyName);
            }
        }

        public Visibility DownloadProgressBarVisibility
        {
            get
            {
                return (DownloadProgressValue > 0 && DownloadProgressValue < 100)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        protected static ToggleButton VisibleItem;

        protected AudioListViewModel()
        {
            MainVm = ((MainViewModel)Application.Current.MainWindow.DataContext);
            MainVm.AccountSignOut += MainVm_AccountSignOut;
            MainVm.AuthorizationSuccess += MainVm_AuthorizationSuccess;
        }

        private void MainVm_AuthorizationSuccess(object sender, System.EventArgs e)
        {
            Task loadAuios = new Task(UpdateAudioList);
            loadAuios.Start();
        }

        private void MainVm_AccountSignOut(object sender, System.EventArgs e)
        {
            Audios.Clear();
            GC.Collect(2,GCCollectionMode.Optimized);
        }

        public static TChildItem FindVisualChild<TChildItem>(DependencyObject obj)
where TChildItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is TChildItem)
                    return (TChildItem)child;
                else
                {
                    TChildItem childOfChild = FindVisualChild<TChildItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public abstract void ChangeListPlayButtonVisibility(object listItem, Visibility visibility);

        public abstract void SwitchListPlayButtonVisibility(object listItem);

        protected abstract void UpdateAudioList();

        private RelayCommand<Uri> _saveAudioFileCommand;

        public RelayCommand<Uri> SaveAudioFile
        {
            get
            {
                return _saveAudioFileCommand
                    ?? (_saveAudioFileCommand = new RelayCommand<Uri>( (url) =>
                    {

                        SaveFileDialog saveDialog = new SaveFileDialog
                        {
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            Filter = "MPEG-1/2/2.5 Layer 3 (*.mp3)|*.mp3"
                        };


                        if (saveDialog.ShowDialog() == true && System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                        {
                            using (WebClient webClient = new WebClient())
                            {
                               
                                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                                webClient.DownloadFileAsync(url, saveDialog.FileName);
                            }
                        }
                    }));
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressValue = e.ProgressPercentage;
            RaisePropertyChanged("DownloadProgressBarVisibility");
        }
    }
}