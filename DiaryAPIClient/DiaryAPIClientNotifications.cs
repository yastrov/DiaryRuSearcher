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
        public NotificationsUnit NotificationsGet()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "notification.get");
            nvc.Add("sid", SID);
            var url = NameValueCollectionToUrlStringUrlEncoding(nvc);
            using (HttpWebResponse response = this._Request(url, "GET", null))
            {
                var r = GetObjectFromJson<NotificationsUnit>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                return r;
            }
        }

        public async Task<NotificationsUnit> NotificationsGetAsync()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "notification.get");
            nvc.Add("sid", SID);
            var url = NameValueCollectionToUrlStringUrlEncoding(nvc);
            using (HttpWebResponse response = await this._RequestAsync(url, "GET", null))
            {
                var r = GetObjectFromJson<NotificationsUnit>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                return r;
            }
        }
    }
}
