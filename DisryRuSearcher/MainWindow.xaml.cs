using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DiaryAPI;
using System.Collections.ObjectModel;
using DiaryRuSearcher.ViewsModels;
using DiaryAPI.JSONResponseClasses;
using System.Diagnostics;
using System.ComponentModel;
using DiaryRuSearcher.Helpers;

namespace DiaryRuSearcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DiaryDataBase _diaryDataBase;

        private IProgress<Double> progress;
        private CancellationTokenSource cancelSource;
        private DiaryAPI.DiaryAPIClient diaryAPIClient = new DiaryAPIClient();

        private ObservableCollection<PostViewModel> postsCollection = null;
        private ObservableCollection<CommentViewModel> commentsCollection = null;
        private ObservableCollection<UmailViewModel> umailsCollection = null;
        private ObservableCollection<UmailFolderViewModel> umailFoldersCollection = null;

        #region Inotify
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        #region DataBinding
        public ObservableCollection<PostViewModel> PostsCollection
        {
            get { return postsCollection; }
            set
            {
                postsCollection = value;
                //It's important for DataBinding via WPF!
                NotifyPropertyChanged("PostsCollection");
            }
        }
        public ObservableCollection<CommentViewModel> CommentsCollection
        {
            get { return commentsCollection; }
            set
            {
                commentsCollection = value;
                //It's important for DataBinding via WPF!
                NotifyPropertyChanged("CommentsCollection");
            }
        }
        public ObservableCollection<UmailViewModel> UmailsCollection
        {
            get { return umailsCollection; }
            set
            {
                umailsCollection = value;
                //It's important for DataBinding via WPF!
                NotifyPropertyChanged("UmailsCollection");
            }
        }
        private Double _progressPercent = 0.0;
        public Double ProgressPercent
        {
            get { return _progressPercent; }
            set
            {
                _progressPercent = value;
                //It's important for DataBinding via WPF!
                NotifyPropertyChanged("ProgressPercent");
            }
        }
        private DateTime _trashDate = DateTime.Now;
        public DateTime TrashDate
        {
            get { return _trashDate; }
            set
            {
                _trashDate = value;
                NotifyPropertyChanged("TrashDate");
            }
        }
        #endregion
        #region IsChecked Flags for DataBinding
        private bool _isDownloadPosts = true;
        public bool IsDownloadPosts
        {
            get { return _isDownloadPosts; }
            set
            {
                _isDownloadPosts = value;
                NotifyPropertyChanged("IsDownloadPosts");
            }
        }
        private bool _isDownloadComments;
        public bool IsDownloadComments
        {
            get { return _isDownloadComments; }
            set
            {
                _isDownloadComments = value;
                NotifyPropertyChanged("IsDownloadComments");
            }
        }
        private bool _isDownloadUmails;
        public bool IsDownloadUmails
        {
            get { return _isDownloadUmails; }
            set
            {
                _isDownloadUmails = value;
                NotifyPropertyChanged("IsDownloadUmails");
            }
        }
        #endregion
        private bool _IsImportantControlEnabled = true;
        public bool IsImportantControlEnabled
        {
            get { return _IsImportantControlEnabled; }
            set
            {
                _IsImportantControlEnabled = value;
                NotifyPropertyChanged("IsImportantControlEnabled");
            }
        }

        #region Version Info fields for DataBind
        public string ProductName
        {
            get { return AboutHelper.AssemblyProduct; }
            set { ; }
        }
        public string ProductVersion
        {
            get { return AboutHelper.AssemblyVersion; }
            set { ; }
        }
        public string ProductCopyright
        {
            get { return AboutHelper.AssemblyCopyright.Replace("Copyright ", String.Empty); }
            set { ; }
        }
        #endregion
        #region TextFields for DataBinding
        private string _userName = string.Empty;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value.Trim();
                NotifyPropertyChanged("UserName");
            }
        }
        private string _diaryUrlForDownload = string.Empty;
        public string DiaryUrlForDownload
        {
            get { return _diaryUrlForDownload; }
            set
            {
                _diaryUrlForDownload = value.Trim();
                NotifyPropertyChanged("DiaryUrlForDownload");
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            //It's important for DataBinding via WPF!
            //DataContext = this; // But we define it in MainWindow XAML
#if DEBUG
            System.Windows.MessageBox.Show("Debug mode!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
#else
            checkNewVersion();
#endif
            _diaryDataBase = new DiaryDataBase();
            BaseViewModel.SetDataBase(_diaryDataBase);
        }

        #region Button clicks
        async private void goButton_Click(object sender, RoutedEventArgs e)
        {
            IsImportantControlEnabled = false;
            await processGoButtonClick();
        }
        async private Task processGoButtonClick()
        {
            bool success = true;
            this.Cursor = Cursors.Wait;
            try
            {
                await Auth();
                diaryAPIClient.TimeoutBetweenRequests = TimeoutBetweenRequests;
                if (IsDownloadPosts)
                {
                    string journalShortname = DiaryUrlForDownload.Trim();
                    if (!String.IsNullOrEmpty(journalShortname))
                    {
                        journalShortname = GetUsernameHelper.GetUsername(journalShortname);
                        await downloadPostsAndComments(journalShortname, IsDownloadComments);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Пожалуйста, введите адрес дневника или сообщества!")
                            .Append(Environment.NewLine)
                            .Append("Данные не могут быть загружены!");
                        System.Windows.MessageBox.Show(sb.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                if (IsDownloadUmails)
                {
                    await downloadUmails();
                }
            }
            catch (OperationCanceledException ex)
            {
                success = false;
                var sb = new StringBuilder();
                sb.Append("Прервано пользователем!")
                    .Append(Environment.NewLine)
                    .Append("Скачанные материалы находятся в базе данных.");
                MessageBox.Show(sb.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DiaryAPIClientException ex)
            {
                success = false;
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                success = false;
                System.Windows.MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                IsImportantControlEnabled = true;
                if (success)
                    System.Windows.MessageBox.Show("Загрузка данных завершена!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void saverButton_Click(object sender, RoutedEventArgs e)
        {
            cancelSource.Cancel();
        }

        #region Search in DB
        private void commentsSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string commentAuthor = commentAuthorTextBox.Text.Trim();
            string commentKeyWord = commentsKeywordTextBox.Text.Trim();
            var result = _diaryDataBase.GetCommentsByAuthorKeyword(commentAuthor, commentKeyWord);
            CommentsCollection = new ObservableCollection<CommentViewModel>(result);
        }
        private void umailSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string umailSender = umailSenderNameTextBox.Text.Trim();
            string umailKeyword = umailKeywordTextBox.Text.Trim();
            string umailTitle = umailTitleTextBox.Text.Trim();
            var result = _diaryDataBase.GetUmailsBySenderTitleKeyword(umailSender, umailTitle, umailKeyword);
            UmailsCollection = new ObservableCollection<UmailViewModel>(result);
        }
        private void postSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string postTitle = postSearchTitleTextBox.Text.Trim();
            string postKeyword = postSearchKeywordTextBox.Text.Trim();
            string postAuthor = postSearchAuthorTextBox.Text.Trim();
            var result = _diaryDataBase.GetPostsByAuthorTitleKeyword(postAuthor, postTitle, postKeyword);
            PostsCollection = new ObservableCollection<PostViewModel>(result);
        }
        #endregion

        private void checkVersionButton_Click(object sender, RoutedEventArgs e)
        {
            checkNewVersion();
        }
        #endregion

        #region for DiaryAPI
        async Task Auth()
        {
            await diaryAPIClient.AuthSecureAsync(UserName.Trim(), passwordBox.SecurePassword);

        }
        async private Task downloadPostsAndComments(string shortname, bool withComments)
        {
            progress = new Progress<Double>(i => ProgressPercent = i);
            cancelSource = new CancellationTokenSource();

            DiarySaverDB saver = new DiarySaverDB();
            saver.BeforeDate = TrashDate;
            saver.CancelTokenSourse = cancelSource;
            var journal = diaryAPIClient.JournalGet("", shortname);
            //In theoretical, you may pass "await" keyword and see the result:
            await diaryAPIClient.AllPostsGetProcessingAsync("diarytype", journal, withComments, saver, progress, cancelSource.Token);
        }
        async private Task downloadUmailFolders()
        {
            progress = new Progress<Double>(i => ProgressPercent = i);
            cancelSource = new CancellationTokenSource();
            List<UmailFolderUnit> folders = await diaryAPIClient.UmailGetFoldersAsync();
            DiarySaverDB saver = new DiarySaverDB();
            saver.BeforeDate = TrashDate;
            saver.CancelTokenSourse = cancelSource;
            foreach (var folder in folders)
            {
                ;//saver.InsertUmailFolder(folder);
            }

        }
        async private Task downloadUmails()
        {

            progress = new Progress<Double>(i => ProgressPercent = i);
            cancelSource = new CancellationTokenSource();
            DiarySaverDB saver = new DiarySaverDB();
            saver.BeforeDate = TrashDate;
            saver.CancelTokenSourse = cancelSource;
            await diaryAPIClient.AllUmailsGetInAllFoldersProcessingAsync(saver, progress, cancelSource.Token);
        }

        #endregion

        /// <summary>
        /// Processing to Click to link on form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Diary Ru Searcher", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Check new Version aviable on GitHub, show MessageBox if yes.
        /// </summary>
        private void checkNewVersion()
        {
            var checker = new NewVersionChecker();
            try
            {
                checker.HasNewVersionAsync().ContinueWith(r =>
                {
                    if (r.Result)
                    {
                        var msr = MessageBox.Show("Доступна новая версия! Открыть в браузере?", "Diary Ru Searcher", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (msr == MessageBoxResult.Yes)
                        {
                            try
                            {
                                Process.Start(NewVersionChecker.BrowserUrl);
                            }
                            catch (Exception ex)
                            {
                                System.Windows.MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (cancelSource != null)
            {
                try
                {
                    cancelSource.Cancel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            base.OnClosing(e);
        }

        #region Sort in ListView
        private void SortCM_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as MenuItem;
            switch (s.Name)
            {
                case "PostsAscendingByDate":
                    PostsCollection = new ObservableCollection<PostViewModel>(PostsCollection.OrderBy(item => Convert.ToInt32(item.Dateline_date)));
                    break;
                case "PostsDescendingByDate":
                    PostsCollection = new ObservableCollection<PostViewModel>(PostsCollection.OrderByDescending(item => Convert.ToInt32(item.Dateline_date)));
                    break;
                case "CommentsAscendingByDate":
                    CommentsCollection = new ObservableCollection<CommentViewModel>(CommentsCollection.OrderBy(item => Convert.ToInt32(item.Dateline)));
                    break;
                case "CommentsDescendingByDate":
                    CommentsCollection = new ObservableCollection<CommentViewModel>(CommentsCollection.OrderByDescending(item => Convert.ToInt32(item.Dateline)));
                    break;
                case "UmailsAscendingByDate":
                    UmailsCollection = new ObservableCollection<UmailViewModel>(UmailsCollection.OrderBy(item => Convert.ToInt32(item.Dateline)));
                    break;
                case "UmailsDescendingByDate":
                    UmailsCollection = new ObservableCollection<UmailViewModel>(UmailsCollection.OrderByDescending(item => Convert.ToInt32(item.Dateline)));
                    break;
            }
        }

        private void ReverseCollectionClick(object sender, RoutedEventArgs e)
        {
            var s = sender as GridViewColumnHeader;
            switch (s.Name)
            {
                case "PostsReverse":
                    PostsCollection = new ObservableCollection<PostViewModel>(PostsCollection.Reverse());
                    break;
                case "CommentsReverse":
                    CommentsCollection = new ObservableCollection<CommentViewModel>(CommentsCollection.Reverse());
                    break;
                case "UmailsReverse":
                    UmailsCollection = new ObservableCollection<UmailViewModel>(UmailsCollection.Reverse());
                    break;
            }
        }
        #endregion
        #region options
        private TimeSpan timeoutBetweenRequests = TimeSpan.FromMilliseconds(2000);
        public TimeSpan TimeoutBetweenRequests
        {
            get { return timeoutBetweenRequests; }
            set
            {
                timeoutBetweenRequests = value;
                NotifyPropertyChanged("TimeoutBetweenRequests");
            }
        }
        private string dataBaseFilePath = DiaryDataBase.DataBasePath;
        public string DataBaseFilePath
        {
            get { return dataBaseFilePath; }
            set
            {
                DiaryDataBase.DataBasePath = value.Trim();
                // Неявная проверка
                dataBaseFilePath = DiaryDataBase.DataBasePath;
                NotifyPropertyChanged("DataBaseFilePath");
            }
        }
        #endregion

        private void CutCommand(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(DataBaseFilePath);
            DataBaseFilePath = string.Empty;
        }

        private void PastCommand(object sender, RoutedEventArgs e)
        {
            DataBaseFilePath = Clipboard.GetText();
        }
        private void CopyCommand(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(DataBaseFilePath);
        }

        private void GetFromFileSaveDialogCommand(object sender, RoutedEventArgs e)
        {
            GetFromFileSaveDialogCommand();
        }
        private void GetFromFileOpenDialogCommand(object sender, RoutedEventArgs e)
        {
            GetFromFileOpenDialogCommand();
        }
        private void GetFromFileSaveDialogCommand()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".sqlt";
            dlg.Filter = "Diary DataBase (*.db,*.sqlite3,*.db3,*.sqlite)|*.db;*.sqlite3;*.db3;*.sqlite|All Files|*.*";
            dlg.Title = "Выберите файл для хранения базы данных:";
            if (!string.IsNullOrEmpty(DataBaseFilePath))
            {
                dlg.InitialDirectory = System.IO.Path.GetDirectoryName(DataBaseFilePath);
                dlg.DefaultExt = System.IO.Path.GetExtension(DataBaseFilePath);
            }
            else
            {
                dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            }

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                DataBaseFilePath = dlg.FileName;
            }
        }
        private void GetFromFileOpenDialogCommand()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".sqlt";
            dlg.Filter = "Diary DataBase (*.db,*.sqlite3,*.db3,*.sqlite)|*.db;*.sqlite3;*.db3;*.sqlite|All Files|*.*";
            dlg.Title = "Выберите файл для хранения базы данных:";
            if (!string.IsNullOrEmpty(DataBaseFilePath))
            {
                dlg.InitialDirectory = System.IO.Path.GetDirectoryName(DataBaseFilePath);
                dlg.DefaultExt = System.IO.Path.GetExtension(DataBaseFilePath);
            }
            else
            {
                dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            }

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                DataBaseFilePath = dlg.FileName;
            }
        }
    }
}
