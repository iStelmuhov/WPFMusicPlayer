using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using GalaSoft.MvvmLight;
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

        public abstract void UpdateAudioList();
    }
}