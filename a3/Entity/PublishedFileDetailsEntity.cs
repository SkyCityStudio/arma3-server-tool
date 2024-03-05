using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class PublishedFileDetailsEntity
    {
        public Response response { get; set; }

        public class Response
        {
            public int result { get; set; }
            public int resultcount { get; set; }
            public Publishedfiledetail[] publishedfiledetails { get; set; }
        }

        public class Publishedfiledetail
        {
            public string publishedfileid { get; set; }
            public int result { get; set; }
            public string creator { get; set; }
            public int creator_app_id { get; set; }
            public int consumer_app_id { get; set; }
            public string filename { get; set; }
            public string file_size { get; set; }
            public string file_url { get; set; }
            public string hcontent_file { get; set; }
            public string preview_url { get; set; }
            public string hcontent_preview { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public int time_created { get; set; }
            public int time_updated { get; set; }
            public int visibility { get; set; }
            public int banned { get; set; }
            public string ban_reason { get; set; }
            public int subscriptions { get; set; }
            public int favorited { get; set; }
            public int lifetime_subscriptions { get; set; }
            public int lifetime_favorited { get; set; }
            public int views { get; set; }
            public Tag[] tags { get; set; }
        }

        public class Tag
        {
            public string tag { get; set; }
        }

    }
}
