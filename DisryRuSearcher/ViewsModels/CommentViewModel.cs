using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryRuSearcher.ViewsModels
{
    class CommentViewModel
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

        public string Postid { get; set; }

        public string Url { get { return this.MakeUrl(); } }

        public string MakeUrl()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://").Append(this.Author_shortname)
            .Append(".diary.ru/p").Append(this.Postid)
            .Append(".htm#").Append(this.Commentid);
            return sb.ToString();
        }

        public CommentViewModel(DiaryAPI.JSONResponseClasses.CommentUnit comment)
        {
            this.Author_avatar = comment.Author_avatar;
            this.Author_jtype = comment.Author_jtype;
            this.Author_shortname = comment.Author_shortname;
            this.Author_title = comment.Author_title;
            this.Author_userid = comment.Author_userid;
            this.Author_username = comment.Author_username;
            this.Can_delete = comment.Can_delete;
            this.Can_edit = comment.Can_edit;
            this.Commentid = comment.Commentid;
            this.Dateline = comment.Dateline;
            this.Message_html = comment.Message_html;
            this.Postid = comment.Postid;
        }
    }
}
