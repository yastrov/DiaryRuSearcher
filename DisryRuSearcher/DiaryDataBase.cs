﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiaryAPI.JSONResponseClasses;
using System.IO;
using DiaryRuSearcher.ViewsModels;
using System.Collections.ObjectModel;

namespace DiaryRuSearcher
{
    class DiaryDataBase
    {
        private string DBPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "DiarySearchDB.sqlt");
        public DiaryDataBase()
        {
            createTables();
        }

        protected void createTables()
        {
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                // Create the tables if they don't exist
                db.CreateTable<DiaryAPI.JSONResponseClasses.UmailFolderUnit>();
                db.CreateTable<DiaryAPI.JSONResponseClasses.UmailUnit>();
                db.CreateTable<DiaryAPI.JSONResponseClasses.PostUnit>();
                db.CreateTable<DiaryAPI.JSONResponseClasses.CommentUnit>();
            }
        }

        #region Posts
        public PostViewModel GetPost(string postId)
        {
            PostViewModel post;
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                var _post = (db.Table<PostUnit>().Where(
                    p => p.Postid.Equals(postId))).Single();
                post = new PostViewModel(_post);
            }
            return post;
        }

        public ObservableCollection<PostViewModel> GetPosts()
        {
            var posts = new ObservableCollection<PostViewModel>();
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                var query = db.Table<PostUnit>().OrderBy(c => c.Dateline_cdate);
                foreach (var _post in query)
                {
                    var unit = new PostViewModel(_post);
                    posts.Add(unit);
                }
            }
            return posts;
        }

        public int InsertPost(PostUnit post)
        {
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
                return db.Insert(post);
        }

        public string SavePost(PostViewModel post)
        {
            string result = string.Empty;
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                string change = string.Empty;
                try
                {
                    var existingPost = db.Table<PostUnit>().Where(
                        p => p.Postid.Equals(post.Postid) ).SingleOrDefault();

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
        public int InsertComment(CommentUnit comment)
        {
            int result = 0;
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
                result = db.Insert(comment);
            return result;
        }
        public CommentViewModel GetComment(string commentId)
        {
            CommentViewModel comment;
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                var _comment = (db.Table<CommentUnit>().Where(
                    p => p.Commentid.Equals(commentId))).Single();
                comment = new CommentViewModel(_comment);
            }
            return comment;
        }
        public ObservableCollection<CommentViewModel> GetComments()
        {
            var comments = new ObservableCollection<CommentViewModel>();
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                var query = db.Table<CommentUnit>().OrderBy(c => c.Commentid);
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
        public int InsertUmail(UmailUnit umail)
        {
            int result = 0;
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
                result = db.Insert(umail);
            return result;
        }
        public UmailViewModel GetUmail(string commentId)
        {
            UmailViewModel comment;
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                var _comment = (db.Table<UmailUnit>().Where(
                    p => p.Umailid.Equals(commentId))).Single();
                comment = new UmailViewModel(_comment);
            }
            return comment;
        }

        public ObservableCollection<UmailViewModel> GetUmails()
        {
            var comments = new ObservableCollection<UmailViewModel>();
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                var query = db.Table<UmailUnit>().OrderBy(c => c.Umailid);
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
        public int InsertUmailFolder(DiaryAPI.JSONResponseClasses.UmailFolderUnit umail)
        {
            int result = 0;
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
                result = db.Insert(umail);
            return result;
        }
        public UmailFolderViewModel GetUmailFolder(string folderId)
        {
            UmailFolderViewModel folder;
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
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
            using (var db = new SQLite.SQLiteConnection(this.DBPath))
            {
                var query = db.Table<UmailFolderUnit>().OrderBy(c => c.Folderid);
                foreach (var _folder in query)
                {
                    var unit = new UmailFolderViewModel(_folder);
                    folders.Add(unit);
                }
            }
            return folders;
        }
        #endregion
    }
}
