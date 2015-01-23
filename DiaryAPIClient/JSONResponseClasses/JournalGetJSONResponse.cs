using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.JSONResponseClasses
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
        public string Userid { get; set; }
        public string Ctime { get; set; }
        public string Shortname { get; set; }
        public string Title { get; set; }
        public string Count_pch { get; set; }
        public string Access { get; set; }
        public string Tags { get; set; }
        public Int64 Posts { get; set; }
        public string Can_write { get; set; }
        public string Count_members { get; set; }
        public string Last_post_id { get; set; }
        public string Last_post { get; set; }
        public string Jtype { get; set; }
    }
}
