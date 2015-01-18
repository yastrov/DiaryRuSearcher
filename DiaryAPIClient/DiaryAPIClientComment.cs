using DiaryAPI.JsonResponseObjects;
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
        public List<CommentUnit> CommentGet(string postid, ulong from)
        {
            List<CommentUnit> result = new List<CommentUnit>();
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "comment.get");
            nvc.Add("sid", _sid);
            nvc.Add("postid", postid);
            //if (from != 0)
             //   nvc.Add("from", from.ToString());
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = this._Request(postString, "GET", null))
            {
                var r = GetObjectFromJson<CommentGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                foreach (KeyValuePair<string, CommentUnit> kvp in r.Comments)
                {
                    kvp.Value.PostId = postid;
                    result.Add(kvp.Value);
                }
            }
            return result;
        }

        public List<CommentUnit> AllCommentsGet(List<PostUnit> posts)
        {
            List<CommentUnit> result = new List<CommentUnit>();
            try
            {
                foreach (PostUnit post in posts)
                {
                    ulong comment_count = post.GetNumOfComments();
                    if (comment_count == 0) continue;
                    ulong i = 0;
                    List<CommentUnit> r;
                    try
                    {
                        r = CommentGet(post.Postid.Trim(), 0);
                        result.AddRange(r);
                        System.Threading.Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                        ;
                    }
                }
            }
            finally
            {
                ;
            }
            return result;
        }
    }
}
