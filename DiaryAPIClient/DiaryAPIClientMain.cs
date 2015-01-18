﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        public const string DiaryAPI_URL = "http://www.diary.ru/api/";
        private const string appkey = ""; // ok
        private const string key = ""; // pk
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

        public DiaryAPIClient()
        {
            ;
        }
    }
}