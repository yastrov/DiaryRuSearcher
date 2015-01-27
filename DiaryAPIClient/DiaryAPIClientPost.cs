using DiaryAPI.Interfaces;
using DiaryAPI.JSONResponseClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        public List<PostUnit> PostGet(string diarytype, string shortname, Int64 from)
        {
            List<PostUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "post.get");
            nvc.Add("sid", _sid);
            nvc.Add("shortname", shortname);
            nvc.Add("type", diarytype);
            if (from != 0)
                nvc.Add("from", from.ToString());
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = this._Request(postString, "GET", null))
            {
                var r = GetObjectFromJson<PostGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                result = r.GetPosts();
            }
            return result;
        }

        public List<PostUnit> AllPostsGet(string diarytype, JournalUnit journal)
        {
            List<PostUnit> result = new List<PostUnit>();
            Int64 post_count = journal.Posts;
            Int64 i = 0;
            List<PostUnit> r;
            while (i < post_count)
            {
                r = PostGet(diarytype, journal.Shortname, i);
                result.AddRange(r);
                System.Threading.Thread.Sleep(1000);
                i += Convert.ToInt64(r.Count());
                //i += r.Count();
            }
            return result;
        }

        #region Processing
        public void AllPostsGetProcessing(string diarytype, JournalUnit journal, IPostCommentsProcessor processor)
        {

            Int64 post_count = journal.Posts;
            int i = 0;
            List<PostUnit> r;
            while (i < post_count)
            {
                r = PostGet(diarytype, journal.Shortname, i);
                foreach (var post in r)
                {
                    processor.ProcessPost(post);
                    System.Threading.Thread.Sleep(1000);
                    AllCommentsGetForPostProcessing(post, processor);
                    System.Threading.Thread.Sleep(1000);
                    //i += Convert.ToUInt64(r.Count());
                    i += r.Count();
                }
            }
        }
        #endregion

        #region Async

        public async Task<List<PostUnit>> PostGetAsync(string diarytype, string shortname, Int64 from)
        {
            List<PostUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "post.get");
            nvc.Add("sid", _sid);
            nvc.Add("shortname", shortname);
            nvc.Add("type", diarytype);
            if (from != 0)
                nvc.Add("from", from.ToString());
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = await this._RequestAsync(postString, "GET", null))
            {
                var r = GetObjectFromJson<PostGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                result = r.GetPosts();
            }
            return result;
        }
        public async Task AllPostsGetProcessingAsync(string diarytype,
                                                JournalUnit journal,
                                                bool withComments,
                                                IPostCommentsProcessor processor,
                                                IProgress<Int64> onProgressPercentChanged,
                                                CancellationToken cancellationToken)
        {
            Int64 post_count = journal.Posts;
            Int64 i = 0;
            List<PostUnit> r;
            while (i < post_count)
            {
                r = await PostGetAsync(diarytype, journal.Shortname, i);
                foreach (var post in r)
                {
                    await Task.Run(() => { processor.ProcessPost(post); });
                    System.Threading.Thread.Sleep(1000);
                    if (withComments)
                        await AllCommentsGetForPostProcessingAsync(post, processor);

                }
                System.Threading.Thread.Sleep(1000);
                var count = r.Count();
                if (count == 0)
                    break;
                i += Convert.ToInt64(count);
                onProgressPercentChanged.Report(i / post_count);
            }
        }
        #endregion
    }
}

