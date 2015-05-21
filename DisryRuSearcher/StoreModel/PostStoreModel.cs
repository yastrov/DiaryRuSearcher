using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace DiaryRuSearcher.StoreModel
{
    public class PostStoreModel:IEqualityComparer<PostStoreModel>
    {
        public string Avatar_path { get; set; }
        public string Author_title { get; set; }
        public string Author_username { get; set; }
        public string Title { get; set; }
        public string Comments_count_data { get; set; }
        public string Author_userid { get; set; }
        public string Subscribed { get; set; }
        //[SQLite.Ignore]
        //public Dictionary<String, String> Tags_data { get; set; }
        public string Can_edit { get; set; }
        public string Author_shortname { get; set; }
        public string Avatarid { get; set; }
        public string No_smile { get; set; }
        [SQLite.PrimaryKey]
        [SQLite.Unique]
        public string Postid { get; set; }
        public string Jaccess { get; set; }
        public string Juserid { get; set; }
        public string Dateline_cdate { get; set; }
        public string Close_access_mode2 { get; set; }
        public string Shortname { get; set; }
        public string No_comments { get; set; }
        public string Close_access_mode { get; set; }
        public string Dateline_date { get; set; }
        public string Access { get; set; }
        public string Message_src { get; set; }
        public string Message_html { get; set; } // No usable?

        public PostStoreModel()
        {
            ;
        }
        public PostStoreModel(DiaryAPI.JSONResponseClasses.PostUnit post)
        {
            this.Access = post.Access;
            this.Author_shortname = post.Shortname;
            this.Author_title = post.Author_title;
            this.Author_userid = post.Author_userid;
            this.Author_username = post.Author_username;
            this.Avatar_path = post.Avatar_path;
            this.Avatarid = post.Avatarid;
            this.Can_edit = post.Can_edit;
            this.Close_access_mode = post.Close_access_mode;
            this.Close_access_mode2 = post.Close_access_mode2;
            this.Comments_count_data = post.Comments_count_data;
            this.Dateline_cdate = post.Dateline_cdate;
            this.Dateline_date = post.Dateline_date;
            this.Jaccess = post.Jaccess;
            this.Juserid = post.Juserid;
            this.Message_src = post.Message_src;
            this.No_comments = post.No_comments;
            this.No_smile = post.No_smile;
            this.Postid = post.Postid;
            this.Shortname = post.Shortname;
            this.Subscribed = post.Subscribed;
            //this.Tags_data = post.Tags_data;
            this.Title = post.Title;
            this.Message_html = post.Message_html;
        }

        public bool Equals(PostStoreModel x, PostStoreModel y)
        {
            return x.Postid.Equals(y.Postid);
        }

        public int GetHashCode(PostStoreModel obj)
        {
            return obj.Postid.GetHashCode();
        }
    }
}
