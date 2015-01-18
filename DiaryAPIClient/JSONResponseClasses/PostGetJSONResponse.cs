using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.JsonResponseObjects
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

        public ulong GetNumOfComments()
        {
            try
            {
                return ulong.Parse(Comments_count_data);
            }
            catch (Exception e)
            {
                return Convert.ToUInt64(0);
            }
        }
    }
}
