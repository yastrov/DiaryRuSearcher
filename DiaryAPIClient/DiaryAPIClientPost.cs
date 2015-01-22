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
        public List<PostUnit> PostGet(string diarytype, string shortname, ulong from)
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
                ulong post_count = journal.Posts;
                ulong i = 0;
                List<PostUnit> r;
                while (i < post_count)
                {
                    r = PostGet(diarytype, journal.Shortname, i);
                    result.AddRange(r);
                    System.Threading.Thread.Sleep(1000);
                    i += Convert.ToUInt64(r.Count());
                }
            return result;
        }

        #region Processing
        public void AllPostsGetProcessing(string diarytype, JournalUnit journal, IPostCommentsProcessor processor)
        {

                ulong post_count = journal.Posts;
                ulong i = 0;
                List<PostUnit> r;
                while (i < post_count)
                {
                    r = PostGet(diarytype, journal.Shortname, i);
                    foreach(var post in r)
                    {
                        processor.ProcessPost(post);
                        System.Threading.Thread.Sleep(1000);
                        AllCommentsGetForPostProcessing(post, processor);
                    System.Threading.Thread.Sleep(1000);
                    i += Convert.ToUInt64(r.Count());
                    }
                }
        }
        #endregion

        #region Async

        public async Task<List<PostUnit>> PostGetAsync(string diarytype, string shortname, ulong from)
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
        public async Task AllPostsGetProcessingAsync(string diarytype, JournalUnit journal, IPostCommentsProcessor processor, IProgress<ulong> onProgressPercentChanged, CancellationToken cancellationToken)
        {
            ulong post_count = journal.Posts;
            ulong i = 0;
            List<PostUnit> r;
            while (i < post_count)
            {
                r = await PostGetAsync(diarytype, journal.Shortname, i);
                foreach (var post in r)
                {
                    await Task.Run(() => {processor.ProcessPost(post);});
                    System.Threading.Thread.Sleep(1000);
                    await AllCommentsGetForPostProcessingAsync(post, processor);
                    
                }
                System.Threading.Thread.Sleep(1000);
                i += Convert.ToUInt64(r.Count());
                onProgressPercentChanged.Report(i / post_count);
            }
        }
        #endregion
    }
}

