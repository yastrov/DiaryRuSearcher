using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.JsonResponseObjects
{
    public class JournalGetJSONResponse : IErrorChecked
    {
        public string Result { get; set; }
        public JournalUnit Journal { get; set; }
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

    public class JournalUnit
    {
        public string userid { get; set; }
        public string ctime { get; set; }
        public string shortname { get; set; }
        public string title { get; set; }
        public string count_pch { get; set; }
        public string access { get; set; }
        public string tags { get; set; }
        public string posts { get; set; }
        public string can_write { get; set; }
        public string count_members { get; set; }
        public string last_post_id { get; set; }
        public string last_post { get; set; }
        public string jtype { get; set; }
    }
}
