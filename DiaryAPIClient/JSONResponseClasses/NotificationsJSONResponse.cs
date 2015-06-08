using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.JSONResponseClasses
{
    public class NotificationsUnit : IErrorChecked
    {
        [JsonProperty("comments_count")]
        public int CommentsCount { get; set; }
        [JsonProperty("discuss_count")]
        public int DiscussCount {get; set; }
        [JsonProperty("umail_count")]
        public int UmailsCount { get; set; }

        [JsonProperty("comments")]
        public Dictionary<string, CommentNotifyInfo> Comments { get; set; }
        [JsonProperty("discuss")]
        public DiscussNotifyInfo[] Discuss { get; set; }
        [JsonProperty("umail")]
        public Dictionary<string, UmailNotifyInfo> Umails { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }

        public Boolean CheckForError()
        {
            return !String.IsNullOrEmpty(this.Error);
        }
        public String GetErrorAsString()
        {
            return this.Error;
        }

        public IEnumerable<CommentNotifyInfo> GetComments()
        {
            return Comments.Values.ToList();
        }
        public IEnumerable<UmailNotifyInfo> GetUmails()
        {
            return Umails.Values.ToList();
        }
        public IEnumerable<DiscussNotifyInfo> GetDiscuss()
        {
            return Discuss.ToList();
        }
    }

    public class CommentNotifyInfo
    {
        [JsonProperty("commentid")]
        public string CommentId { get; set; }
        [JsonProperty("message_txt")]
        public string MessageTxt { get; set; }
        [JsonProperty("postid")]
        public string PostId { get; set; }
    }

    public class DiscussNotifyInfo
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("dateline")]
        public string Dateline { get; set; }
        [JsonProperty("journal_name")]
        public string JournalName { get; set; }
        [JsonProperty("juserid")]
        public string JuserId { get; set; }
        [JsonProperty("postid")]
        public string PostId { get; set; }
    }

    public class UmailNotifyInfo
    {
        [JsonProperty("dateline")]
        public string Dateline { get; set; }
        [JsonProperty("message_txt")]
        public string MessageTxt { get; set; }
        [JsonProperty("folder")]
        public string FolderId { get; set; }
        [JsonProperty("from_userid")]
        public string FromUserid { get; set; }
        [JsonProperty("from_username")]
        public string FromUsername { get; set; }
        [JsonProperty("title")]
        public string  Title { get; set; }
        [JsonProperty("umailid")]
        public string UmailId { get; set; }
    }
}
