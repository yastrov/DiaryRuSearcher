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
            nvc.Add("shortname", shortname);
            nvc.Add("userid", userid);
            string postString = string.Join("&", nvc.AllKeys.Where(_key =>
                !string.IsNullOrWhiteSpace(nvc[_key]))
                .Select(_key =>
                    string.Format("{0}={1}", System.Net.WebUtility.UrlEncode(_key), System.Net.WebUtility.UrlEncode(nvc[_key]))));
            using (HttpWebResponse response = this._Request(DiaryAPI_URL + "?" + postString, "GET", null))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new WebException(response.StatusDescription);
                using (Stream oStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(oStream);
                    var r = JsonConvert.DeserializeObject<JournalGetJSONResponse>(reader.ReadToEnd());
                    return r.Journal;
                }
            }
        }

        
    }
}
