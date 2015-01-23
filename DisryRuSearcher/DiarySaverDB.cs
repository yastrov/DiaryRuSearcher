using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiaryAPI.Interfaces;
using DiaryAPI.JSONResponseClasses;

namespace DiaryRuSearcher
{
    class DiarySaverDB: DiaryDataBase, IPostCommentsProcessor, IUmailProcessor
    {
        public DiarySaverDB()
        {
            base.createTables();
        }
        public void ProcessComment(CommentUnit comment)
        {
            InsertComment(comment);
        }
        public void ProcessPost(PostUnit post)
        {
            InsertPost(post);
        }
        public void ProcessUmail(UmailUnit umail)
        {
            InsertUmail(umail);
        }
    }
}
