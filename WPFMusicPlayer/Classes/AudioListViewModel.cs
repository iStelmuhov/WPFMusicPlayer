using System.Collections.ObjectModel;
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

        protected ToggleButton VisibleItem;

        protected AudioListViewModel()
        {
            MainVm = ((MainViewModel)Application.Current.MainWindow.DataContext);
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

        public virtual void ChangeListPlayButtonVisibility(object listItem, Visibility visibility)
        {}

        public virtual void SwitchListPlayButtonVisibility(object listItem)
        { }
    }
}