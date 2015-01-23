using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryRuSearcher.ViewsModels
{
    class UmailViewModel
    {
        public string From_username { get; set; }
        public string Message_html { get; set; }
        public string Read { get; set; }
        public string Dateline { get; set; }
        public string Title { get; set; }
        public string Umailid { get; set; }
        public string From_userid { get; set; }
        public string No_smilies { get; set; }

        public UmailViewModel(DiaryAPI.JSONResponseClasses.UmailUnit umail)
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

        public static DiaryAPI.JSONResponseClasses.UmailUnit FromViewModel(UmailViewModel view)
        {
            var umail = new DiaryAPI.JSONResponseClasses.UmailUnit();
            umail.From_username = view.From_username;
            umail.Message_html = view.Message_html;
            umail.Read = view.Read;
            umail.Dateline = view.Dateline;
            umail.Title = view.Title;
            umail.Umailid = view.Umailid;
            umail.From_userid = view.From_userid;
            umail.No_smilies = view.No_smilies;
            return umail;
        }
    }
    
}
