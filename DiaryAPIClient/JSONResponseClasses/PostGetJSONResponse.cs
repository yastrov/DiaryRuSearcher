﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.JSONResponseClasses
{
    public class PostGetJSONResponse : IErrorChecked
    {
        public Dictionary<string, PostUnit> Posts { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }

        public Boolean CheckForError()
        {
            return !String.IsNullOrEmpty(this.Error);
        }
        public String GetErrorAsString()
        {
            return this.Error;
        }

        public List<PostUnit> GetPosts()
        {
            List<PostUnit> result = new List<PostUnit>();
            foreach (KeyValuePair<string, PostUnit> kvp in this.Posts)
            {
                kvp.Value.Postid = kvp.Value.Postid.Trim();
                result.Add(kvp.Value);
            }
            return result;
        }
    }

    public class PostUnit
    {
        public string Avatar_path { get; set; }
        public string Author_title { get; set; }
        public string Author_username { get; set; }
        public string Title { get; set; }
        public string Comments_count_data { get; set; }
        public string Author_userid { get; set; }
        public string Subscribed { get; set; }
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
        public string Message_html { get; set; } // No usable?
        public string Url { get { return this.MakeUrl(); } }
        public string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Message_html)) return this.Message_html;
                if (!string.IsNullOrEmpty(this.Message_src)) return this.Message_src;
                return string.Empty;
            }
        }

        public int GetNumOfComments()
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
    }
}
