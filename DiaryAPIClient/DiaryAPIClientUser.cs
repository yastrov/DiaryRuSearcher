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
        public UserUnit UserGet(string userid)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "user.get");
            nvc.Add("sid", _sid);
            if (!String.IsNullOrEmpty(userid))
                nvc.Add("userid", userid);
            var url = NameValueCollectionToUrlStringUrlEncoding(nvc);
            using (HttpWebResponse response = this._Request(url, "GET", null))
            {
                var r = GetObjectFromJson<UserGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                return r.User;
            }
        }

        public async Task<UserUnit> UserGetAsync (string userid)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "user.get");
            nvc.Add("sid", _sid);
            if (!String.IsNullOrEmpty(userid))
                nvc.Add("userid", userid);
            var url = NameValueCollectionToUrlStringUrlEncoding(nvc);
            using (HttpWebResponse response = await this._RequestAsync(url, "GET", null))
            {
                var r = GetObjectFromJson<UserGetJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                return r.User;
            }
        }
    }
}
