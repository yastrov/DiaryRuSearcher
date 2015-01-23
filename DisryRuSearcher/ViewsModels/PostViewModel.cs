using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryRuSearcher.ViewsModels
{
    class PostViewModel
    {
        public string Avatar_path { get; set; }
        public string Author_title { get; set; }
        public string Author_username { get; set; }
        public string Title { get; set; }
        public string Comments_count_data { get; set; }
        public string Author_userid { get; set; }
        public string Subscribed { get; set; }
        [SQLite.Ignore]
        public Dictionary<String, String> Tags_data { get; set; }
        public string Can_edit { get; set; }
        public string Author_shortname { get; set; }
        public string Avatarid { get; set; }
        public string No_smile { get; set; }
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

        public string Url { get { return this.MakeUrl(); } }

        public Int64 GetNumOfComments()
        {
            try
            {
                return int.Parse(Comments_count_data);
                //return ulong.Parse(Comments_count_data);
            }
            catch (Exception e)
            {
                return 0;// return Convert.ToUInt64(0);
            }
        }

        public string MakeUrl()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://").Append(this.Author_shortname)
                .Append(".diary.ru/p").Append(this.Postid).Append(".htm");
            return sb.ToString();
        }

        public PostViewModel(DiaryAPI.JSONResponseClasses.PostUnit post)
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
            this.Tags_data = post.Tags_data;
            this.Title = post.Title;
        }

        public static DiaryAPI.JSONResponseClasses.PostUnit ModelFromView(PostViewModel view)
        {
            var post = new DiaryAPI.JSONResponseClasses.PostUnit();
            post.Access = view.Access;
            post.Author_shortname = view.Shortname;
            post.Author_title = view.Author_title;
            post.Author_userid = view.Author_userid;
            post.Author_username = view.Author_username;
            post.Avatar_path = view.Avatar_path;
            post.Avatarid = view.Avatarid;
            post.Can_edit = view.Can_edit;
            post.Close_access_mode = view.Close_access_mode;
            post.Close_access_mode2 = view.Close_access_mode2;
            post.Comments_count_data = view.Comments_count_data;
            post.Dateline_cdate = view.Dateline_cdate;
            post.Dateline_date = view.Dateline_date;
            post.Jaccess = view.Jaccess;
            post.Juserid = view.Juserid;
            post.Message_src = view.Message_src;
            post.No_comments = view.No_comments;
            post.No_smile = view.No_smile;
            post.Postid = view.Postid;
            post.Shortname = view.Shortname;
            post.Subscribed = view.Subscribed;
            post.Tags_data = view.Tags_data;
            post.Title = view.Title;
            return post;
        }
    }
}
