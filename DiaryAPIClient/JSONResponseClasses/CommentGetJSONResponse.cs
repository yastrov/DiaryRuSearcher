using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.JsonResponseObjects
{
    public class CommentGetJSONResponse : IErrorChecked
    {
        public Dictionary<string, CommentUnit> Comments { get; set; }
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

    public class CommentUnit
    {
        public string Can_edit { get; set; }
        public string Author_title { get; set; }
        public string Author_shortname { get; set; }
        public string Author_avatar { get; set; }
        public string Author_username { get; set; }
        public string Commentid { get; set; }
        public string Dateline { get; set; }
        public string Author_userid { get; set; }
        public string Shortname { get; set; }
        public string Message_html { get; set; }
        public string Can_delete { get; set; }
        public string Author_jtype { get; set; }

        public string PostId { get; set; }

        
    }

}
