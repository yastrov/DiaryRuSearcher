using DiaryAPI.JSONResponseClasses;
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
        public JournalUnit JournalGet(string userid, string shortname)
        {
            NameValueCollection nvc = new NameValueCollection();
            //Dictionary<string, string> nvc = new Dictionary<string, string>();

            nvc.Add("method", "journal.get");
            nvc.Add("sid", _sid);
            if(!String.IsNullOrEmpty(shortname))
            nvc.Add("shortname", shortname);
            if (!String.IsNullOrEmpty(userid))
            nvc.Add("userid", userid);
            var url = NameValueCollectionToUrlStringUrlEncoding(nvc);
            using (HttpWebResponse response = this._Request(url, "GET", null))
            {
                var r = GetObjectFromJson<JournalGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                return r.Journal;
            }
        }

        public async Task<JournalUnit> JournalGetAsync(string userid, string shortname)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc.Add("method", "journal.get");
            nvc.Add("sid", _sid);
            if (!String.IsNullOrEmpty(shortname))
                nvc.Add("shortname", shortname);
            if (!String.IsNullOrEmpty(userid))
                nvc.Add("userid", userid);
            var url = NameValueCollectionToUrlStringUrlEncoding(nvc);
            using (HttpWebResponse response = await this._RequestAsync(url, "GET", null))
            {
                var r = GetObjectFromJson<JournalGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                return r.Journal;
            }
        }
    }
}
