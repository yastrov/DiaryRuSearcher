using DiaryRuSearcher.StoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryRuSearcher.ViewsModels
{
    public class UmailViewModel : BaseViewModel
    {
        public string From_username { get; set; }
        public string Message_html { get; set; }
        public string Read { get; set; }
        public string Dateline { get; set; }
        public string Title { get; set; }
        public string Umailid { get; set; }
        public string From_userid { get; set; }
        public string No_smilies { get; set; }

        public string Url { get { return this.MakeUrl(); } }

        public string MakeUrl()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://www.diary.ru/u-mail/read/?u_id=")
                .Append(this.Umailid);
            return sb.ToString();
        }
        public string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Message_html)) return this.Message_html;
                return string.Empty;
            }
        }
        public string MessageForView
        {
            get
            {

                if (!string.IsNullOrEmpty(this.Message_html))
                {
                    int len = (this.Message_html.Length > 256) ? 256 : this.Message_html.Length;
                    return this.Message_html.Substring(0, len);
                }
                return string.Empty;
            }
        }

        public string DateTimeValue
        {
            get { return ConvertFromUnixTimestamp(Convert.ToDouble(this.Dateline)).ToLongDateString(); }
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

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

        public UmailViewModel(UmailStoreModel umail)
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
