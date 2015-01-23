using DiaryAPI.Interfaces;
using DiaryAPI.JSONResponseClasses;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        public List<CommentUnit> CommentGet(string postid, int from)
        {
            List<CommentUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "comment.get");
            nvc.Add("sid", _sid);
            nvc.Add("postid", postid);
            if (from != 0)
                nvc.Add("from", from.ToString());
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = this._Request(postString, "GET", null))
            {
                var r = GetObjectFromJson<CommentGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                result = r.GetComments(postid);
            }
            return result;
        }

        public List<CommentUnit> AllCommentsGetForPost(PostUnit post)
        {
            if (post.GetNumOfComments() == 0)
                return new List<CommentUnit>();
            return CommentGet(post.Postid.Trim(), 0);
        }

        public List<CommentUnit> AllCommentsGet(List<PostUnit> posts)
        {
            List<CommentUnit> result = new List<CommentUnit>();
                foreach (PostUnit post in posts)
                {
                    result.AddRange(AllCommentsGetForPost(post));
                    System.Threading.Thread.Sleep(1000);
                }
            return result;
        }

        #region Processing
        public void AllCommentsGetForPostProcessing(PostUnit post, ICommentProcessor processor)
        {
            if (post.GetNumOfComments() == 0) return;
            List<CommentUnit> r = CommentGet(post.Postid.Trim(), 0);
            foreach(var c in r)
            {
                processor.ProcessComment(c);
            }
        }
        #endregion

        #region Async

        public async Task<List<CommentUnit>> CommentGetAsync(string postid, int from)
        {
            List<CommentUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "comment.get");
            nvc.Add("sid", _sid);
            nvc.Add("postid", postid);
            if (from != 0)
                nvc.Add("from", from.ToString());
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = await this._RequestAsync(postString, "GET", null))
            {
                var r = GetObjectFromJson<CommentGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                result = r.GetComments(postid);
            }
            return result;
        }
        public async Task AllCommentsGetForPostProcessingAsync(PostUnit post, ICommentProcessor processor)
        {
            if (post.GetNumOfComments() == 0) return;
            List<CommentUnit> r = await CommentGetAsync(post.Postid.Trim(), 0);
            foreach (var c in r)
            {
                processor.ProcessComment(c);
            }
        }
        #endregion
    }
}
