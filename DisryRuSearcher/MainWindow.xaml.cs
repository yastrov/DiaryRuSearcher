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

        private ObservableCollection<PostViewModel> postsCollection = null;
        private ObservableCollection<CommentViewModel> commentsCollection = null;
        private ObservableCollection<UmailViewModel> umailsCollection = null;
        private ObservableCollection<UmailFolderViewModel> umailFoldersCollection = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        async private void goButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                await diaryAPIClient.AuthSecureAsync(loginBox.Text, passwordBox.SecurePassword);
                progress = new Progress<Int64>(i => progressBar.Value = i);
                cancelSource = new CancellationTokenSource();
                try
                {
                    DiarySaverDB saver = new DiarySaverDB();
                    var journal = diaryAPIClient.JournalGet("", "");
                    //In theoretical, you may pass "await" keyword and see the result:
                    await diaryAPIClient.AllPostsGetProcessingAsync("diarytype", journal, saver, progress, cancelSource.Token);
                }
                catch (OperationCanceledException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            catch(DiaryAPIClientException ex)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void saverButton_Click(object sender, RoutedEventArgs e)
        {
            cancelSource.Cancel();
        }
    }
}
