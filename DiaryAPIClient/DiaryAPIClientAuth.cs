using DiaryAPI.JsonResponseObjects;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        public void Auth(string username, string password)
        {
            NameValueCollection nvc = new NameValueCollection();
            //nvc.Add("username", Convert(username));
            nvc.Add("username", username);
            nvc.Add("password", MakeDiaryPassword(password));
            nvc.Add("method", "user.auth");
            nvc.Add("appkey", appkey);
            string postString = NameValueCollectionToParamString(nvc);
            using (HttpWebResponse response = this._Request("POST", ConvertToWinByte(postString)))
            {
                var r = GetObjectFromJson<AuthJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                _sid = r.Sid;
            }
        }

        public async Task AuthSecureAsync(string username, System.Security.SecureString password)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("username", username);
            nvc.Add("password", MakeDiaryPassword(password));
            nvc.Add("method", "user.auth");
            nvc.Add("appkey", appkey);
            string postString = NameValueCollectionToParamString(nvc);
            using (HttpWebResponse response = await this._RequestAsync("POST", ConvertToWinByte(postString)).ConfigureAwait(false))
            {
                var r = GetObjectFromJson<AuthJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                _sid = r.Sid;
            }
        }
    }
}
