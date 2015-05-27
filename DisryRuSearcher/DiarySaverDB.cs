using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiaryAPI.Interfaces;
using DiaryAPI.JSONResponseClasses;
using System.Threading;
using DiaryRuSearcher.StoreModel;

namespace DiaryRuSearcher
{
    class DiarySaverDB: DiaryDataBase, IPostCommentsProcessor, IUmailProcessor
    {
        private CancellationTokenSource _CancellationTokenSource;
        private bool _dateFlag = false;
        private DateTime _trashholdDate = DateTime.Now;
        public CancellationTokenSource CancelTokenSourse
        {
            get { return _CancellationTokenSource; }
            set { _CancellationTokenSource = value; }
        }
        public DateTime BeforeDate
        {
            get { return _trashholdDate; }
            set {
                _trashholdDate = value;
                if (value.Equals(DateTime.Now))
                    _dateFlag = false;
                else _dateFlag = true;
            }
        }

        public DiarySaverDB()
        {
            base.createTables();
        }
        public void ProcessComment(CommentUnit comment)
        {
            InsertComment(new CommentStoreModel(comment));
        }
        public void ProcessPost(PostUnit post)
        {
            if (_dateFlag)
            {
                if (ConvertFromUnixTimestamp(post.Dateline_date)
                    .CompareTo(_trashholdDate) < 0)
                    _CancellationTokenSource.Cancel();
            }
            InsertPost(new PostStoreModel(post));
            foreach(var item in post.Tags_data)
            {
                InsertTagWithoutReplace(new TagStoreModel(item.Key, item.Value));
                InsertRelation(new TagRelationStoreModel(item.Key, post.Postid));
            }
        }
        public void ProcessUmail(UmailUnit umail)
        {
            if (_dateFlag)
            {
                if (ConvertFromUnixTimestamp(umail.Dateline)
                    .CompareTo(_trashholdDate) < 0)
                    _CancellationTokenSource.Cancel();
            }
            InsertUmail(new UmailStoreModel(umail));
        }

        private static DateTime ConvertFromUnixTimestamp(string timestamp)
        {
            return ConvertFromUnixTimestamp(Convert.ToDouble(timestamp));
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
