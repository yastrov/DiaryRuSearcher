using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.JSONResponseClasses
{
    [JsonObject]
    public class UserGetJSONResponse : IErrorChecked
    {
        public string Result { get; set; }
        [JsonProperty("user")]
        public UserUnit User { get; set; }

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

    public class UserUnit
    {
        public string journal { get; set; }
        public string about { get; set; }
        public string sex { get; set; }
        public string joindate { get; set; }
        public string tw { get; set; }
        public string birthday { get; set; }
        public string country { get; set; }
        [JsonProperty("yo")]
        public string AgeAsStr { get; set; }
        public string journal_title { get; set; }
        [SQLite.PrimaryKey]
        public string userid { get; set; }
        [SQLite.Ignore]
        public Dictionary<int, String> Readers2 { get; set; }
        public string city { get; set; }
        public string[] readers { get; set; }
        public string avatar { get; set; }
        public string shortname { get; set; }
        [JsonProperty("community.member")]
        public string[] communitymember { get; set; }
        [SQLite.Ignore]
        [JsonProperty("community.member2")]
        public Dictionary<int, String> communitymember2 { get; set; }
        public string[] fav { get; set; }
        public int readers_count { get; set; }
        [JsonProperty("community.member_count")]
        public int communitymember_count { get; set; }
        public string username { get; set; }
        public string joindate2 { get; set; }
        public string locked { get; set; }
        public int favs_count { get; set; }
        [SQLite.Ignore]
        public Dictionary<int, String> favs2 { get; set; }
        public string education { get; set; }
        public string birthday2 { get; set; }
        [SQLite.Ignore]
        public Dictionary<int, String> interest { get; set; }
        public string region { get; set; }

        [SQLite.Ignore]
        public string Url { get { return this.MakeUrl(); } }
        public string MakeUrl()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://www.diary.ru/member/?").Append(this.userid);
            return sb.ToString();
        }
    }


}
