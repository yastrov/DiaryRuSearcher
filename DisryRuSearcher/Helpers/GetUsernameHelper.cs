using System.Text.RegularExpressions;

namespace DiaryRuSearcher.Helpers
{
    public class GetUsernameHelper
    {
        private static Regex[] myReg = {new Regex(@"^(https?://)?www.diary.ru/~(.+?)/?$"),
                                        new Regex(@"^(https?://)?(.+?).diary.ru")};
        
        public static string GetUsername(string url)
        {
            foreach (var reg in myReg)
            {
                Match match = reg.Match(url);
                if (match.Success)
                {
                    return match.Groups[2].Value;
                }
            }
            return url;
        }
    }
}
