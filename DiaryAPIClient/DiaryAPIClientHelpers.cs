using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        public string NameValueCollectionToParamString(System.Collections.Specialized.NameValueCollection nvc)
        {
            return string.Join("&", nvc.AllKeys.Where(_key =>
                !string.IsNullOrWhiteSpace(nvc[_key]))
                .Select(_key =>
                    string.Format("{0}={1}", _key, nvc[_key])));
        }

        public string NameValueCollectionToUrlString(System.Collections.Specialized.NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DiaryAPI_URL).Append("?");
            var s = string.Join("&", nvc.AllKeys.Where(_key =>
                !string.IsNullOrWhiteSpace(nvc[_key]))
                .Select(_key =>
                    string.Format("{0}={1}", _key, nvc[_key])));
            sb.Append(s);
            return sb.ToString();
        }

        #region encoding
        public string ConvertStringToWinString(string str)
        {
            Encoding srcEncodingFormat = Encoding.Default;
            Encoding dstEncodingFormat = Encoding.GetEncoding("windows-1251");
            byte[] originalByteString = srcEncodingFormat.GetBytes(str);
            byte[] convertedByteString = Encoding.Convert(srcEncodingFormat,
            dstEncodingFormat, originalByteString);
            return dstEncodingFormat.GetString(convertedByteString);
        }

        public byte[] ConvertToWinByte(string str)
        {
            Encoding srcEncodingFormat = Encoding.Default;
            Encoding dstEncodingFormat = Encoding.GetEncoding("windows-1251");
            byte[] originalByteString = srcEncodingFormat.GetBytes(str);
            return Encoding.Convert(srcEncodingFormat,
            dstEncodingFormat, originalByteString);
        }
        #endregion

        #region password
        public static string MakeDiaryPassword(System.Security.SecureString password)
        {
            using (password)
            {
                string pass = new System.Net.NetworkCredential(string.Empty, password).Password;
                StringBuilder sb = new StringBuilder();
                MD5 md5 = new MD5CryptoServiceProvider();
                sb.Append(key).Append(pass);
                byte[] checkSum = md5.ComputeHash(Encoding.Default.GetBytes(sb.ToString()));
                return BitConverter.ToString(checkSum).Replace("-", String.Empty).ToLower();
            }
        }

        public static string MakeDiaryPassword(String password)
        {
                StringBuilder sb = new StringBuilder();
                MD5 md5 = new MD5CryptoServiceProvider();
                sb.Append(key).Append(password);
                byte[] checkSum = md5.ComputeHash(Encoding.Default.GetBytes(sb.ToString()));
                return BitConverter.ToString(checkSum).Replace("-", String.Empty).ToLower();
        }
        #endregion

        public static T GetObjectFromJson<T>(HttpWebResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new WebException(response.StatusDescription);
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            using (Stream oStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(oStream);
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd(), settings);
            }
        }
    }
}
