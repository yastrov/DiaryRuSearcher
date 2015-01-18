using DiaryAPI.JsonResponseObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        public List<PostUnit> PostGet(string diarytype, string shortname, ulong from)
        {
            List<PostUnit> result = new List<PostUnit>();
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "post.get");
            nvc.Add("sid", _sid);
            nvc.Add("shortname", shortname);
            nvc.Add("type", diarytype);
            nvc.Add("from", from.ToString());
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = this._Request(postString, "GET", null))
            {
                var r = GetObjectFromJson<PostGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);

                    foreach (KeyValuePair<string, PostUnit> kvp in r.Posts)
                    {
                        kvp.Value.postid = kvp.Value.Postid.Trim();
                        result.Add(kvp.Value);
                    }
                
            }
            return result;
        }

        public List<PostUnit> AllPostsGet(string diarytype, JournalUnit journal)
        {
            List<PostUnit> result = new List<PostUnit>();
            try
            {
                ulong post_count = ulong.Parse(journal.posts);
                ulong i = 0;
                List<PostUnit> r;
                while (i < post_count)
                {
                    r = PostGet(diarytype, journal.shortname, i);
                    result.AddRange(r);
                    System.Threading.Thread.Sleep(1000);
                    i += Convert.ToUInt64(r.Count());
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
