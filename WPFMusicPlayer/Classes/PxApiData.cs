using System.Collections.Generic;

namespace WPFMusicPlayer.Classes
{
    public class Image
    {
        public int size { get; set; }
        public string url { get; set; }
        public string https_url { get; set; }
        public string format { get; set; }
    }

    public class Default
    {
        public string https { get; set; }
    }

    public class Large
    {
        public string https { get; set; }
    }

    public class Small
    {
        public string https { get; set; }
    }

    public class Tiny
    {
        public string https { get; set; }
    }

    public class Avatars
    {
        public Default @default { get; set; }
        public Large large { get; set; }
        public Small small { get; set; }
        public Tiny tiny { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public int usertype { get; set; }
        public string fullname { get; set; }
        public string userpic_url { get; set; }
        public string userpic_https_url { get; set; }
        public string cover_url { get; set; }
        public int upgrade_status { get; set; }
        public bool store_on { get; set; }
        public int affection { get; set; }
        public Avatars avatars { get; set; }
        public int followers_count { get; set; }
    }

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

    public class Filters
    {
        public bool category { get; set; }
        public bool exclude { get; set; }
    }

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