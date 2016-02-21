using System;
using System.IO;
using System.Linq;

using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace WPFMusicPlayer.Classes
{
    public class WebImage: ViewModelBase
    {

        public const string ImagePropertyName = "Image";
        private Uri _image;

        public Uri Image
        {
            get
            {
                return _image;;
            }

            set
            {
                if (_image == value)
                {
                    return;
                }

                _image = value;
                RaisePropertyChanged(ImagePropertyName);
            }
        }

        public WebImage(string url)
        {
            Image=new Uri(url);
        }
        /**********************************************************************************************************/

        public static async Task<WebImage> GetAlbumCoverFromSpotify(string artistName,string audioName)
        {

            var client = new HttpClient();
            var baseUrl = GetBaseUrl(audioName, artistName);

            var spotifyData = await client.GetStringAsync(baseUrl);

            SpotifyApiData apiData = JsonConvert.DeserializeObject<SpotifyApiData>(spotifyData);

            if (apiData.tracks.items.Count >= 1)
            {
                if (apiData.tracks.items[0].album.images[0]!=null)
                    return new WebImage(apiData.tracks.items[0].album.images[0].url);
            }
            return new WebImage("http://www.4shared.com/images/no-cover-d1-music.png");
        }

        private static string GetBaseUrl( string audioName, string artistName)
        {
            var Url = "https://api.spotify.com/v1/search?" +
                      "q={0}" +
                      "+" +
                      "artist%3A{1}" +
                      "&type=track" +
                      "&limit=1" +
                      "&offset=1";
            var baseUrl = string.Format(Url, StringConverToWeb(audioName), StringConverToWeb(artistName));

            return baseUrl;
        }

        private static string StringConverToWeb(string str)
        {
            return str.Replace(" ", "+");
        }

        /**********************************************************************************************************/

        public static async Task<WebImage> GetMostPopularPhotoFrom500px(string apiKey,int imageSize,int pageNumber=1,int imgNumber=1)
        {
            var client =new HttpClient();
            var baseUrl =
                string.Format("https://api.500px.com/v1/photos?feature=popular&image_size={0}&consumer_key={1}&page={2}&rpp={3}",
                    imageSize, apiKey,pageNumber, imgNumber);

            var pxData = await client.GetStringAsync(baseUrl);

            if (pxData != null)
            {
                var apiData = JsonConvert.DeserializeObject<PxApiData>(pxData);
                if (apiData == null) throw new ArgumentNullException(nameof(apiData));

                return new WebImage(apiData.photos.ElementAt(--imgNumber).images.ElementAt(0).url);
            }
            throw new ArgumentNullException(nameof(pxData));
        }
    }
}