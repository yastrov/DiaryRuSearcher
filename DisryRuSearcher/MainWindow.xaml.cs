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

namespace DiaryRuSearcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IProgress<Int64> progress;
        private CancellationTokenSource cancelSource;
        private DiaryAPI.DiaryAPIClient diaryAPIClient = new DiaryAPIClient();

        private static ObservableCollection<PostViewModel> postsCollection = null;
        private ObservableCollection<CommentViewModel> commentsCollection = null;
        private ObservableCollection<UmailViewModel> umailsCollection = null;
        private ObservableCollection<UmailFolderViewModel> umailFoldersCollection = null;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            System.Windows.MessageBox.Show("Debug mode!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
#else
            var checker = new NewVersionChecker();
            checker.HasNewVersionAsync().ContinueWith(r =>
            {
                if (r.Result)
                {
                    var msr = MessageBox.Show("Доступна новая версия! Открыть в браузере?", this.Title, MessageBoxButton.YesNo, MessageBoxImage.Information);
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
#endif
            productNameLabel.Content = AboutHelper.AssemblyProduct;
            versionLabel.Content = AboutHelper.AssemblyVersion;
            copyrightLabel.Content = AboutHelper.AssemblyCopyright.Replace("Copyright ", String.Empty);
        }

        #region Button clicks
        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            processGoButtonClick().Start();
        }
        async private Task processGoButtonClick()
        {
            try
            {
                await Auth();
                if (downloadPostsCheckBox.IsChecked ?? false)
                {
                    string journalShortname = journalShortNameTextBox.Text.Trim();
                    if (!String.IsNullOrEmpty(journalShortname))
                    {
                        journalShortname = journalShortname.Replace("http://", String.Empty);
                        journalShortname = journalShortname.Replace("diary.ru", String.Empty);
                        journalShortname = journalShortname.Replace("/", String.Empty);
                    }
                    await downloadPostsAndComments(journalShortname, downloadCommentsCheckBox.IsChecked ?? false);
                }
                if (downloadUmailsCheckBox.IsChecked ?? false)
                {
                    await downloadUmails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void saverButton_Click(object sender, RoutedEventArgs e)
        {
            cancelSource.Cancel();
        }

        private void commentsSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string commentAuthor = commentAuthorTextBox.Text.Trim();
            string commentKeyWord = commentsKeywordTextBox.Text.Trim();
            commentsCollection = new DiaryDataBase().GetCommentsByAuthorKeyword(commentAuthor, commentKeyWord);
            commentsListView.ItemsSource = commentsCollection;
        }
        private void umailSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string umailSender = umailSenderNameTextBox.Text.Trim();
            string umailKeyword = umailKeywordTextBox.Text.Trim();
            string umailTitle = umailTitleTextBox.Text.Trim();
            umailsCollection = new DiaryDataBase().GetUmailsBySenderTitleKeyword(umailSender, umailTitle, umailKeyword);
            umailsListView.ItemsSource = umailsCollection;
        }

        #endregion

        #region for DiaryAPI
        async Task Auth()
        {
            try
            {
                await diaryAPIClient.AuthSecureAsync(loginBox.Text.Trim(), passwordBox.SecurePassword);
            }
            catch (DiaryAPIClientException ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        async private Task downloadPostsAndComments(string shortname, bool withComments)
        {
            try
            {
                progress = new Progress<Int64>(i => progressBar.Value = i);
                cancelSource = new CancellationTokenSource();
                try
                {
                    DiarySaverDB saver = new DiarySaverDB();
                    var journal = diaryAPIClient.JournalGet("", shortname);
                    //In theoretical, you may pass "await" keyword and see the result:
                    await diaryAPIClient.AllPostsGetProcessingAsync("diarytype", journal, withComments, saver, progress, cancelSource.Token);
                }
                catch (OperationCanceledException ex)
                {
                    MessageBox.Show("Прервано пользователем!", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (DiaryAPIClientException ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        async private Task downloadUmailFolders()
        {
            try
            {
                progress = new Progress<Int64>(i => progressBar.Value = i);
                cancelSource = new CancellationTokenSource();
                try
                {
                    List<UmailFolderUnit> folders = await diaryAPIClient.UmailGetFoldersAsync();
                    DiarySaverDB saver = new DiarySaverDB();
                    foreach (var folder in folders)
                    {
                        saver.InsertUmailFolder(folder);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    MessageBox.Show("Прервано пользователем!", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (DiaryAPIClientException ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        async private Task downloadUmails()
        {
            try
            {
                progress = new Progress<Int64>(i => progressBar.Value = i);
                cancelSource = new CancellationTokenSource();
                try
                {
                    DiarySaverDB saver = new DiarySaverDB();
                    await diaryAPIClient.AllUmailsGetInAllFoldersProcessingAsync(saver, progress, cancelSource.Token);
                }
                catch (OperationCanceledException ex)
                {
                    MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (DiaryAPIClientException ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

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

        private void postSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string postTitle = postSearchTitleTextBox.Text.Trim();
            string postKeyword = postSearchKeywordTextBox.Text.Trim();
            string postAuthor = postSearchAuthorTextBox.Text.Trim();
            postsCollection = new DiaryDataBase().GetPostsByAuthorTitleKeyword(postAuthor, postTitle, postKeyword);
            postsListView.ItemsSource = postsCollection;
        }
    }
}
