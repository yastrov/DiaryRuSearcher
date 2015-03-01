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

namespace DiaryRuSearcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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
        #endregion
        #region IsChecked Flags for DataBinding
        private bool _isDownloadPosts = true;
        public bool IsDownloadPosts
        {
            get { return _isDownloadPosts;}
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
                _userName = value;
                NotifyPropertyChanged("UserName");
            }
        }
        private string _diaryUrlForDownload = string.Empty;
        public string DiaryUrlForDownload
        {
            get { return _diaryUrlForDownload; }
            set
            {
                _diaryUrlForDownload = value;
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
        }

        #region Button clicks
        async private void goButton_Click(object sender, RoutedEventArgs e)
        {
            goButton.IsEnabled = false;
            await processGoButtonClick();
        }
        async private Task processGoButtonClick()
        {
            try
            {
                await Auth();
                if (IsDownloadPosts)
                {
                    string journalShortname = DiaryUrlForDownload.Trim();
                    if (!String.IsNullOrEmpty(journalShortname))
                    {
                        journalShortname = journalShortname.Replace("http://", String.Empty);
                        journalShortname = journalShortname.Replace("diary.ru", String.Empty);
                        journalShortname = journalShortname.Replace("/", String.Empty);
                    }
                    await downloadPostsAndComments(journalShortname, IsDownloadComments);
                }
                if (IsDownloadUmails)
                {
                    await downloadUmails();
                }
            }
            catch (OperationCanceledException ex)
            {
                MessageBox.Show("Прервано пользователем!", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DiaryAPIClientException ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                goButton.IsEnabled = true;
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
            CommentsCollection = new DiaryDataBase().GetCommentsByAuthorKeyword(commentAuthor, commentKeyWord);
        }
        private void umailSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string umailSender = umailSenderNameTextBox.Text.Trim();
            string umailKeyword = umailKeywordTextBox.Text.Trim();
            string umailTitle = umailTitleTextBox.Text.Trim();
            UmailsCollection = new DiaryDataBase().GetUmailsBySenderTitleKeyword(umailSender, umailTitle, umailKeyword);
        }
        private void postSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string postTitle = postSearchTitleTextBox.Text.Trim();
            string postKeyword = postSearchKeywordTextBox.Text.Trim();
            string postAuthor = postSearchAuthorTextBox.Text.Trim();
            PostsCollection = new DiaryDataBase().GetPostsByAuthorTitleKeyword(postAuthor, postTitle, postKeyword);
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
            foreach (var folder in folders)
            {
                saver.InsertUmailFolder(folder);
            }

        }
        async private Task downloadUmails()
        {

            progress = new Progress<Double>(i => ProgressPercent = i);
            cancelSource = new CancellationTokenSource();
            DiarySaverDB saver = new DiarySaverDB();
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
                    ;
                }
            }
        }
    }
}
