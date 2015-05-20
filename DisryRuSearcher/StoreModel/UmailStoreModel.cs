using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace DiaryRuSearcher.StoreModel
{
    public class UmailStoreModel
    {
        public string From_username { get; set; }
        public string Message_html { get; set; }
        public string Read { get; set; }
        public string Dateline { get; set; }
        public string Title { get; set; }
        [SQLite.PrimaryKey]
        [SQLite.Unique]
        public string Umailid { get; set; }
        public string From_userid { get; set; }
        public string No_smilies { get; set; }

        public UmailStoreModel()
        {;}
        public UmailStoreModel(DiaryAPI.JSONResponseClasses.UmailUnit umail)
        {
            this.From_username = umail.From_username;
            this.Message_html = umail.Message_html;
            this.Read = umail.Read;
            this.Dateline = umail.Dateline;
            this.Title = umail.Title;
            this.Umailid = umail.Umailid;
            this.From_userid = umail.From_userid;
            this.No_smilies = umail.No_smilies;
        }
    }
}
