using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        #region Requests
        /// <summary>
        /// Do Abstract Request.
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestMethod">GET or POST or smth</param>
        /// <param name="content">byte array content</param>
        /// <returns>Response object</returns>
        protected HttpWebResponse _Request(String requestMethod, byte[] content)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(DiaryAPI_URL);
            request.ProtocolVersion = new Version(1, 0);
            request.Method = requestMethod;

            if (this._timeout != 0)
                request.Timeout = this._timeout;
            request.AllowAutoRedirect = false;
            request.UserAgent = _userAgent;
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "windows-1251,utf-8;q=0.7,*;q=0.3");
            if (content != null)
            {
                request.ContentType = _contentType;
                request.ContentLength = content.LongLength;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(content, 0, content.Length);
                }
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            request = null;
            return response;
        }

        /// <summary>
        /// Do Abstract Request.
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestMethod">GET or POST or smth</param>
        /// <param name="content">byte array content</param>
        /// <returns>Response object</returns>
        protected HttpWebResponse _Request(String url, String requestMethod, byte[] content)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.ProtocolVersion = new Version(1, 0);
            request.Method = requestMethod;

            if (this._timeout != 0)
                request.Timeout = this._timeout;
            request.AllowAutoRedirect = false;
            request.UserAgent = _userAgent;
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "windows-1251,utf-8;q=0.7,*;q=0.3");
            if (content != null)
            {
                request.ContentType = _contentType;
                request.ContentLength = content.LongLength;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(content, 0, content.Length);
                }
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            request = null;
            return response;
        }

        /// <summary>
        /// Do Abstract Request.
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestMethod">GET or POST or smth</param>
        /// <param name="content">byte array content</param>
        /// <returns>Response object</returns>
        protected async Task<HttpWebResponse> _RequestAsync(String requestMethod, byte[] content)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(DiaryAPI_URL);
            request.ProtocolVersion = new Version(1, 0);
            request.Method = requestMethod;

            if (this._timeout != 0)
                request.Timeout = this._timeout;
            request.AllowAutoRedirect = false;
            request.UserAgent = _userAgent;
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "windows-1251,utf-8;q=0.7,*;q=0.3");
            if (content != null)
            {
                request.ContentType = _contentType;
                request.ContentLength = content.LongLength;
                using (Stream newStream = await request.GetRequestStreamAsync())
                {
                    newStream.Write(content, 0, content.Length);
                }
            }
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            request = null;

            return response;
        }

        /// <summary>
        /// Do Abstract Request.
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestMethod">GET or POST or smth</param>
        /// <param name="content">byte array content</param>
        /// <returns>Response object</returns>
        protected async Task<HttpWebResponse> _RequestAsync(String url, String requestMethod, byte[] content)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.ProtocolVersion = new Version(1, 0);
            request.Method = requestMethod;

            if (this._timeout != 0)
                request.Timeout = this._timeout;
            request.AllowAutoRedirect = false;
            request.UserAgent = _userAgent;
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "windows-1251,utf-8;q=0.7,*;q=0.3");
            if (content != null)
            {
                request.ContentType = _contentType;
                request.ContentLength = content.LongLength;
                using (Stream newStream = await request.GetRequestStreamAsync())
                {
                    newStream.Write(content, 0, content.Length);
                }
            }
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            request = null;

            return response;
        }
        #endregion
    }
}
