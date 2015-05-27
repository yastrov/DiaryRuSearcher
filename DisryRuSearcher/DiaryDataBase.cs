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
                db.CreateTable<TagStoreModel>();
                db.CreateTable<TagRelationStoreModel>();
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
        public IEnumerable<CommentViewModel> GetCommentsByPostId(string PostId)
        {
            IEnumerable<CommentViewModel> comments;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                comments = db.Table<CommentStoreModel>()
                    .Where(item => item.Postid.Equals(PostId))
                    .OrderByDescending(c => c.Commentid)
                    .Select(item => new CommentViewModel(item))
                    .ToList();
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

        #region Tags
        public IEnumerable<TagViewModel> GetTags(){
            IEnumerable<TagViewModel> result;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
               result = db.Table<TagStoreModel>()
                   .OrderBy(c => c.Description)
                   .Select(item => new TagViewModel(item))
                   .ToList();

            }
            return result;
        }
        public TagViewModel GetTag(string TagId)
        {
            TagViewModel post = null;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var _post = (db.Table<TagStoreModel>().Where(
                   p => p.TagId.Equals(TagId))).Single();
                if (_post != null)
                    post = new TagViewModel(_post);
            }
            return post;
        }

        public int InsertTagWithoutReplace(TagStoreModel tag)
        {
            int result = -1;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var existingPost = db.Table<TagStoreModel>().Where(
                       p => p.TagId.Equals(tag.TagId)).SingleOrDefault();
                if (existingPost == null)
                {
                    result = db.Insert(tag);
                }
            }
            return result;
        }

        public int InsertRelation(TagRelationStoreModel model)
        {
            int result = -1;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                var existingPost = db.Table<TagRelationStoreModel>().Where(
                       p => p.TagId.Equals(model.TagId) && p.PostId.Equals(model.PostId)).SingleOrDefault();
                if (existingPost == null)
                {
                    result = db.Insert(model);
                }
            }
            return result;
        }
        #endregion

        #region Search by...
        public IEnumerable<CommentViewModel> GetCommentsByAuthorKeyword(string commentAuthor, string commentKeyWord)
        {
            Regex regex;
            IEnumerable<CommentViewModel> result = new List<CommentViewModel>();
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                if (!string.IsNullOrEmpty(commentAuthor) && string.IsNullOrEmpty(commentKeyWord))
                {
                    regex = new Regex(commentAuthor);
                    result = db.Table<CommentStoreModel>().AsEnumerable()
                        .Where(c => regex.IsMatch(c.Author_username))
                        .OrderByDescending(c => c.Dateline).Select(item => new CommentViewModel(item)).ToList();
                }
                if (!string.IsNullOrEmpty(commentKeyWord) && string.IsNullOrEmpty(commentAuthor))
                {
                    regex = new Regex(commentKeyWord);
                    result = db.Table<CommentStoreModel>().AsEnumerable()
                        .Where(c => regex.IsMatch(c.Message_html))
                        .OrderByDescending(c => c.Dateline).Select(item => new CommentViewModel(item)).ToList();
                }
                if (!string.IsNullOrEmpty(commentKeyWord) && !string.IsNullOrEmpty(commentAuthor))
                {
                    regex = new Regex(commentAuthor);
                    var r1 = new Regex(commentKeyWord);
                    result = db.Table<CommentStoreModel>().AsEnumerable()
                        .Where(c => regex.IsMatch(c.Author_username))
                        .Where(c => r1.IsMatch(c.Message_html))
                        .OrderByDescending(c => c.Dateline).Select(item => new CommentViewModel(item)).ToList();
                }
                return result;
            }
        }

        public IEnumerable<UmailViewModel> GetUmailsBySenderTitleKeyword(string umailSender, string title, string umailKeyword)
        {
            Stack<IEnumerable<UmailStoreModel>> r = new Stack<IEnumerable<UmailStoreModel>>(3);
            IEnumerable<UmailStoreModel> q;
            Regex regexp;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                if (!string.IsNullOrEmpty(umailSender))
                {
                    regexp = new Regex(umailSender);
                    q = db.Table<UmailStoreModel>().AsEnumerable()
                        .Where(c => regexp.IsMatch(c.From_username)).ToList();
                        //.OrderByDescending(c => c.Dateline);
                    r.Push(q);
                    
                }
                if (!string.IsNullOrEmpty(title))
                {
                    regexp = new Regex(title);
                    q = db.Table<UmailStoreModel>().AsEnumerable()
                        .Where(c => regexp.IsMatch(c.Title))
                        .ToList();//.OrderByDescending(C => C.Dateline);
                    r.Push(q);
                    
                }
                if (!string.IsNullOrEmpty(umailKeyword))
                {
                    regexp = new Regex(umailKeyword);
                    q = db.Table<UmailStoreModel>().AsEnumerable()
                        .Where(c => regexp.IsMatch(c.Message_html)).ToList();
                    r.Push(q);
                        //.OrderByDescending(C => C.Dateline);
                    
                }
            }
            #region Intersect
            if (r.Count == 0)
                return new List<UmailViewModel>();
            else
            {
                while (r.Count > 1)
                {
                    var i = r.Pop();
                    i = Enumerable.Intersect(i, r.Pop(), i.First());
                    r.Push(i);
                }
                return r.Pop().OrderByDescending(item => item.Dateline)
                    .Select(item => new UmailViewModel(item));
            }
            #endregion
        }
        #endregion

        public IEnumerable<PostViewModel> GetPostsByAuthorTitleKeyword(string postAuthor, string postTitle, string postKeyword, IEnumerable<TagViewModel> tags)
        {
            Stack<IEnumerable<PostStoreModel>> r = new Stack<IEnumerable<PostStoreModel>>(3);
            Regex regexp;
            using (var db = new SQLite.SQLiteConnection(DBPath))
            {
                if (!string.IsNullOrEmpty(postAuthor))
                {
                    regexp = new Regex(postAuthor);
                    var q = db.Table<PostStoreModel>().AsEnumerable()
                        .Where(c => regexp.IsMatch(c.Author_username)).ToList();
                    r.Push(q);
                }
                if (!string.IsNullOrEmpty(postTitle))
                {
                    regexp = new Regex(postTitle);
                    var q = db.Table<PostStoreModel>().AsEnumerable()
                        .Where(c => regexp.IsMatch(c.Title)).ToList();
                    r.Push(q);
                }
                if (!string.IsNullOrEmpty(postKeyword))
                {
                    regexp = new Regex(postKeyword);
                    var q= db.Table<PostStoreModel>().AsEnumerable()
                        .Where(c => regexp.IsMatch(c.Message_html)).ToList();
                    r.Push(q);
                }
                if(tags != null && tags.Count() > 0)
                {
                    var tagIdList = tags.Select(item => item.TagId);
                    var postIdList = db.Table<TagRelationStoreModel>()
                        .Where(item => tagIdList.Contains(item.TagId))
                        .Select(item=>item.PostId).ToList();
                    var q = db.Table<PostStoreModel>()
                        .Where(item => postIdList.Contains(item.Postid))
                        .ToList();
                    r.Push(q);
                }
            }
            if (r.Count == 0)
                return new List<PostViewModel>();
            else
            {
                while (r.Count > 1)
                {
                    var i = r.Pop();
                    i = Enumerable.Intersect(i, r.Pop(), i.First());
                    r.Push(i);
                }
                return r.Pop().OrderByDescending(item => item.Dateline_date)
                    .Select(item => new PostViewModel(item));
            }
        }
    }
}
