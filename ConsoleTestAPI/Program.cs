using DiaryAPI;
using DiaryAPI.JSONResponseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleTestAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            DiaryAPIClient dapi = new DiaryAPIClient();
            dapi.Auth("", "");
            JournalUnit journal = dapi.JournalGet(null, null);
            System.Console.WriteLine(dapi.SID);
            List<PostUnit> posts = dapi.AllPostsGet("diary", journal);
            foreach(var p in posts)
            {
                System.Console.WriteLine("{0} - {1} - {2}", p.Postid, p.Comments_count_data, p.GetNumOfComments());
            }
            List<CommentUnit> comments = dapi.AllCommentsGet(posts);
            Console.WriteLine("Count of comments: {0}", comments.Count());
            foreach(var c in comments)
            {
                System.Console.WriteLine("-------------");
                System.Console.WriteLine(c.Message_html);
                System.Console.WriteLine(c.Author_avatar);
            }
        }
    }
}
