using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DiaryRuSearcher.ViewsModels;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using DiaryRuSearcher.StoreModel;

namespace DiaryRuSearcher
{
    public class DiaryDataBase
    {
        private static string defaultDBName = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "DiarySearchDB.db");
        private static string DBPath = defaultDBName;
        public static string DataBasePath {
            get { return DBPath; }
            set
            {
                if (Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                {
                    DBPath = value;
                }
                else
                {
                    DBPath = defaultDBName;
                }
            }
    }
        public DiaryDataBase()
        {
            createTables();
        }

        protected void createTables()
        {
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                // Create the tables if they don't exist
                db.CreateTable<UmailStoreModel>();
                db.CreateTable<PostStoreModel>();
                db.CreateTable<CommentStoreModel>();
            }
        }

        #region Posts
        public PostViewModel GetPost(string postId)
        {
            PostViewModel post=null;
            try
            {
                using (var db = new SQLite.SQLiteConnection(DBPath))
                {
                    var _post = (db.Table<PostStoreModel>().Where(
                        p => p.Postid.Equals(postId))).Single();
                    if(_post != null)
                    post = new PostViewModel(_post);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return post;
        }

        public ObservableCollection<PostViewModel> GetPosts()
        {
            var posts = new ObservableCollection<PostViewModel>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var query = db.Table<PostStoreModel>().OrderBy(c => c.Dateline_cdate);
                foreach (var _post in query)
                {
                    var unit = new PostViewModel(_post);
                    posts.Add(unit);
                }
            }
            return posts;
        }

        public int InsertPost(PostStoreModel post)
        {
            int result = -1;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var existingPost = db.Table<PostStoreModel>().Where(
                       p => p.Postid.Equals(post.Postid)).SingleOrDefault();
                if (existingPost == null)
                {
                    result = db.Insert(post);
                }
            }
            return result;
        }

        public string SavePost(PostViewModel post)
        {
            string result = string.Empty;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                string change = string.Empty;
                try
                {
                    var existingPost = db.Table<PostStoreModel>().Where(
                        p => p.Postid.Equals(post.Postid)).SingleOrDefault();

                    if (existingPost != null)
                    {
                        int success = db.Update(existingPost);
                    }
                    else
                    {
                        int success = db.Insert(PostViewModel.ModelFromView(post));
                    }
                    result = "Success";
                }
                catch (Exception ex)
                {
                    result = "This customer was not saved.";
                }
            }
            return result;
        }
        #endregion

        #region Comment
        public int InsertComment(CommentStoreModel comment)
        {
            int result = 0;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var existingC = db.Table<CommentStoreModel>().Where(
                       p => p.Commentid.Equals(comment.Commentid)).SingleOrDefault();
                if (existingC == null)
                {
                    result = db.Insert(comment);
                }
            }
            return result;
        }
        public CommentViewModel GetComment(string commentId)
        {
            CommentViewModel comment;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var _comment = (db.Table<CommentStoreModel>().Where(
                    p => p.Commentid.Equals(commentId))).Single();
                comment = new CommentViewModel(_comment);
            }
            return comment;
        }
        public ObservableCollection<CommentViewModel> GetComments()
        {
            var comments = new ObservableCollection<CommentViewModel>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var query = db.Table<CommentStoreModel>().OrderByDescending(c => c.Commentid);
                foreach (var _comment in query)
                {
                    var unit = new CommentViewModel(_comment);
                    comments.Add(unit);
                }
            }
            return comments;
        }
        #endregion
        #region Umail
        public int InsertUmail(UmailStoreModel umail)
        {
            int result = 0;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var existingU = db.Table<UmailStoreModel>().Where(
                       p => p.Umailid.Equals(umail.Umailid)).SingleOrDefault();
                if (existingU == null)
                {
                    result = db.Insert(umail);
                }
            }
            return result;
        }
        public UmailViewModel GetUmail(string commentId)
        {
            UmailViewModel comment;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var _comment = (db.Table<UmailStoreModel>().Where(
                    p => p.Umailid.Equals(commentId))).Single();
                comment = new UmailViewModel(_comment);
            }
            return comment;
        }

        public ObservableCollection<UmailViewModel> GetUmails()
        {
            var comments = new ObservableCollection<UmailViewModel>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var query = db.Table<UmailStoreModel>().OrderByDescending(c => c.Umailid);
                foreach (var _comment in query)
                {
                    var unit = new UmailViewModel(_comment);
                    comments.Add(unit);
                }
            }
            return comments;
        }
        #endregion

        #region UmailFolder
        /*
        public int InsertUmailFolder(DiaryAPI.JSONResponseClasses.UmailFolderUnit umail)
        {
            int result = 0;
            using (var db = new SQLite.SQLiteConnection(DBPath))
                result = db.Insert(umail);
            return result;
        }
        public UmailFolderViewModel GetUmailFolder(string folderId)
        {
            UmailFolderViewModel folder;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var _folder = (db.Table<UmailFolderUnit>().Where(
                    p => p.Folderid.Equals(folderId))).Single();
                folder = new UmailFolderViewModel(_folder);
            }
            return folder;
        }

        public ObservableCollection<UmailFolderViewModel> GetUmailFolders()
        {
            var folders = new ObservableCollection<UmailFolderViewModel>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var query = db.Table<UmailFolderUnit>().OrderByDescending(c => c.Folderid);
                foreach (var _folder in query)
                {
                    var unit = new UmailFolderViewModel(_folder);
                    folders.Add(unit);
                }
            }
            return folders;
        }
         * */
        #endregion

        #region Search by...
        public ObservableCollection<CommentViewModel> GetCommentsByAuthorKeyword(string commentAuthor, string commentKeyWord)
        {
            var result = new ObservableCollection<CommentViewModel>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                if (!string.IsNullOrEmpty(commentAuthor))
                {
                    var query = db.Table<CommentStoreModel>().Where(c => c.Author_username.Equals(commentAuthor)).OrderByDescending(c => c.Dateline);
                    foreach (var _c in query)
                    {
                        var unit = new CommentViewModel(_c);
                        result.Add(unit);
                    }
                }
                if (!string.IsNullOrEmpty(commentKeyWord))
                {
                    Regex regex = new Regex(commentKeyWord);
                    var query = db.Table<CommentStoreModel>().AsEnumerable().Where(c => regex.IsMatch(c.Message_html)).OrderByDescending(c => c.Dateline);
                    foreach (var _c in query)
                    {
                        var unit = new CommentViewModel(_c);
                        result.Add(unit);
                    }
                }
            }
            return result;
        }

        public ObservableCollection<UmailViewModel> GetUmailsBySenderTitleKeyword(string umailSender, string title, string umailKeyword)
        {
            var result = new ObservableCollection<UmailViewModel>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                if (!string.IsNullOrEmpty(umailSender))
                {
                    var query = db.Table<UmailStoreModel>().Where(c => c.From_username.Equals(umailSender)).OrderByDescending(c => c.Dateline);
                    foreach (var _c in query)
                    {
                        var unit = new UmailViewModel(_c);
                        result.Add(unit);
                    }
                }
                if (!string.IsNullOrEmpty(title))
                {
                    var query = db.Table<UmailStoreModel>().Where(c => c.Title.Contains(title)).OrderBy(c => c.Dateline).OrderByDescending(C => C.Dateline);
                    foreach (var _c in query)
                    {
                        var unit = new UmailViewModel(_c);
                        result.Add(unit);
                    }
                }
                if (!string.IsNullOrEmpty(umailKeyword))
                {
                    Regex regexp = new Regex(umailKeyword);
                    var query = db.Table<UmailStoreModel>().AsEnumerable().Where(c => regexp.IsMatch(c.Message_html)).OrderByDescending(C => C.Dateline);
                    foreach (var _c in query)
                    {
                        var unit = new UmailViewModel(_c);
                        result.Add(unit);
                    }
                }
            }
            return result;
        }
        #endregion

        public ObservableCollection<PostViewModel> GetPostsByAuthorTitleKeyword(string postAuthor, string postTitle, string postKeyword)
        {
            var result = new ObservableCollection<PostViewModel>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                if (!string.IsNullOrEmpty(postAuthor))
                {
                    var query = db.Table<PostStoreModel>().Where(c => c.Author_username.Equals(postAuthor)).OrderByDescending(c => c.Dateline_date);
                    foreach (var _c in query)
                    {
                        var unit = new PostViewModel(_c);
                        result.Add(unit);
                    }
                }
                if (!string.IsNullOrEmpty(postTitle))
                {
                    var query = db.Table<PostStoreModel>().Where(c => c.Title.Contains(postTitle)).OrderByDescending(c => c.Dateline_date);
                    foreach (var _c in query)
                    {
                        var unit = new PostViewModel(_c);
                        result.Add(unit);
                    }
                }
                if (!string.IsNullOrEmpty(postKeyword))
                {
                    Regex regexp = new Regex(postKeyword);
                    var query = db.Table<PostStoreModel>().AsEnumerable().Where(c => regexp.IsMatch(c.Message_html)).OrderByDescending(c => c.Dateline_date);
                    foreach (var _c in query)
                    {
                        var unit = new PostViewModel(_c);
                        result.Add(unit);
                    }
                }
            }
            return result;
        }
    }
}
