using System.Collections.Generic;

namespace WPFMusicPlayer.Classes
{
    public class Photo
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string camera { get; set; }
        public string lens { get; set; }
        public string focal_length { get; set; }
        public string iso { get; set; }
        public string shutter_speed { get; set; }
        public string aperture { get; set; }
        public int times_viewed { get; set; }
        public double rating { get; set; }
        public int status { get; set; }
        public string created_at { get; set; }
        public int category { get; set; }
        public object location { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public string taken_at { get; set; }
        public int hi_res_uploaded { get; set; }
        public bool for_sale { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int votes_count { get; set; }
        public int favorites_count { get; set; }
        public int comments_count { get; set; }
        public bool nsfw { get; set; }
        public int sales_count { get; set; }
        public object for_sale_date { get; set; }
        public double highest_rating { get; set; }
        public string highest_rating_date { get; set; }
        public int license_type { get; set; }
        public int converted { get; set; }
        public int collections_count { get; set; }
        public int crop_version { get; set; }
        public bool privacy { get; set; }
        public bool profile { get; set; }
        public string image_url { get; set; }
        public List<Image> images { get; set; }
        public string url { get; set; }
        public int positive_votes_count { get; set; }
        public int converted_bits { get; set; }
        public bool watermark { get; set; }
        public string image_format { get; set; }
        public User user { get; set; }
        public bool licensing_requested { get; set; }
    }
}