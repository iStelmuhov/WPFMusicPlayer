using System.Collections.Generic;

namespace WPFMusicPlayer.Classes
{
    public class PxApiData
    {
        public int current_page { get; set; }
        public int total_pages { get; set; }
        public int total_items { get; set; }
        public List<Photo> photos { get; set; }
        public Filters filters { get; set; }
        public string feature { get; set; }
    }
}