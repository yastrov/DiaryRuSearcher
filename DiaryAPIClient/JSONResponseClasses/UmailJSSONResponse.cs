using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.JSONResponseClasses
{
    public class UmailJSONResponse : IErrorChecked
    {
        public string Result { get; set; }
        public Dictionary<string, UmailUnit> Umail { get; set; }
        public Int64 Count { get; set; }
        public string Error { get; set; }

        public Boolean CheckForError()
        {
            return !String.IsNullOrEmpty(this.Error);
        }
        public String GetErrorAsString()
        {
            return this.Error;
        }

        public List<UmailUnit> GetUmails()
        {
            List<UmailUnit> result = new List<UmailUnit>();
            foreach (KeyValuePair<string, UmailUnit> kvp in this.Umail)
            {
                result.Add(kvp.Value);
            }
            return result;
        }
    }

    public class UmailUnit
    {
        public string From_username { get; set; }
        public string Message_html { get; set; }
        public string Read { get; set; }
        public string Dateline { get; set; }
        public string Title { get; set; }
        [SQLite.PrimaryKey]
        public string Umailid { get; set; }
        public string From_userid { get; set; }
        public string No_smilies { get; set; }
    }

    //Umail folders
    public class UmailFoldersJSONResponse : IErrorChecked
    {
        public string Result { get; set; }
        public Dictionary<string, UmailFolderUnit> Folders2 { get; set; }
        public int Count { get; set; }
        public string Error { get; set; }

        public Boolean CheckForError()
        {
            return !String.IsNullOrEmpty(this.Error);
        }
        public String GetErrorAsString()
        {
            return this.Error;
        }

        public List<UmailFolderUnit> GetFolders()
        {
            List<UmailFolderUnit> result = new List<UmailFolderUnit>();
            foreach (KeyValuePair<string, UmailFolderUnit> kvp in this.Folders2)
            {
                kvp.Value.Folderid = kvp.Key;
                result.Add(kvp.Value);
            }
            return result;
        }
    }

    public class UmailFolderUnit
    {
        public string Name { get; set; }
        public int Count { get; set; }
        [SQLite.PrimaryKey]
        public string Folderid { get; set; }
    }
}
