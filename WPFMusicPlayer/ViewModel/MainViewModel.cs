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
using MaterialDesignThemes.Wpf;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.Attachments;
using WPFMusicPlayer.Classes;
using WPFMusicPlayer.Enums;

namespace WPFMusicPlayer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public const ulong APPID = 5233775;

        private ApiAuthParams _autorizeAuthParams;
        public VkApi VkApi;


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _autorizeAuthParams=new ApiAuthParams();

            VkApi=new VkApi();

            _autorizeAuthParams.Login = "shyrovec@rambler.ru";
            _autorizeAuthParams.Password = "Stelmuhov";
            _autorizeAuthParams.ApplicationId = APPID;
            _autorizeAuthParams.Settings = Settings.All;


            VkApi.Authorize(_autorizeAuthParams);
        }
    }
}