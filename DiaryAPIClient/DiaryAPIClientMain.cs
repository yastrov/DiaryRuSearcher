using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        public const string DiaryAPI_URL = "http://www.diary.ru/api/";
        #region API keys
        private const string appkey = ""; // ok
        private const string key = ""; // pk
        #endregion
        private TimeSpan timeoutBetweenRequests = TimeSpan.FromMilliseconds(1000);
        public TimeSpan TimeoutBetweenRequests
        {
            get { return timeoutBetweenRequests; }
            set { timeoutBetweenRequests = value; }
        }
        private string _sid;
        public string SID
        {
            get { return _sid; }
            set { _sid = value; }
        }

        private int _timeout = 5000;
        public int Timeout
        {
            get { return _timeout; }
            set { if (value >= 0) _timeout = value; }
        }
        private static string _userAgent = @"DiaryRuAPI";

        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        private static string _contentType = "application/x-www-form-urlencoded; charset=windows-1251";

        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        private System.Net.IWebProxy _proxy = null;
        public System.Net.IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        public DiaryAPIClient()
        {
            ;
        }
    }
}
