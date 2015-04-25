using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using DiaryAPI.JSONResponseClasses;
using System.Diagnostics;

namespace WomanSearch
{
    public class WomanView
    {
        public string journal { get; set; }
        public string sex { get; set; }
        public string joindate { get; set; }
        public string birthday { get; set; }
        public string country { get; set; }
        public string AgeAsStr { get; set; }
        [SQLite.PrimaryKey]
        public string userid { get; set; }

        public string city { get; set; }

        public string shortname { get; set; }

        public string username { get; set; }


        public string birthday2 { get; set; }

        public string region { get; set; }

        [SQLite.Ignore]
        public string Url { get { return this.MakeUrl(); } }
        public string MakeUrl()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("http://www.diary.ru/member/?").Append(this.userid);
            return sb.ToString();
        }

        public WomanView(UserUnit user)
        {
            this.userid = user.userid;
            this.sex = user.sex;
            this.region = user.region;
            this.city = user.city;
            this.birthday2 = user.birthday2;
            this.username = user.username;
            this.AgeAsStr = user.AgeAsStr;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CancellationTokenSource cancelSource = null;
        private DiaryAPI.DiaryAPIClient diaryAPIClient = new DiaryAPI.DiaryAPIClient();
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
        private ObservableCollection<WomanView> _womans = new ObservableCollection<WomanView>();
        public ObservableCollection<WomanView> Womans
        {
            get { return _womans; }
            set
            {
                _womans = value;
                //It's important for DataBinding via WPF!
                NotifyPropertyChanged("Womans");
            }
        }
        #endregion


        #region TextFields for DataBinding
        private string _userName = "Юрий Рэйн";
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyPropertyChanged("UserName");
            }
        }


        private string _fromUser = "3314085";
        public string FromUser
        {
            get { return _fromUser; }
            set
            {
                _fromUser = value.Trim();
                NotifyPropertyChanged("FromUser");
            }
        }
        private string _toUser = "3314000";
        public string ToUser
        {
            get { return _toUser.Trim(); }
            set
            {
                _toUser = value;
                NotifyPropertyChanged("ToUser");
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            this.ToUser = _toUser;
            this.FromUser = "3314085";
            UserName = _userName;
        }

        private async void goButton_Click(object sender, RoutedEventArgs e)
        {
            goButton.IsEnabled = false;
            cancelSource = new CancellationTokenSource();
            await processGoButtonClick();
        }

        private void saverButton_Click(object sender, RoutedEventArgs e)
        {
            if (cancelSource != null)
                cancelSource.Cancel();
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
        }

        async private Task processGoButtonClick()
        {
            bool success = true;
            this.Cursor = Cursors.Wait;
            List<WomanView> list = null;

            try
            {
                await Auth();
                var user = await diaryAPIClient.UserGetAsync("1348587");
                //MessageBox.Show(user.username);
                list = await downloadUsers(FromUser, ToUser, cancelSource.Token);
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
            catch (DiaryAPI.DiaryAPIClientException ex)
            {
                success = false;
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //catch (Exception ex)
            //{
            //    success = false;
            //    System.Windows.MessageBox.Show(ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            finally
            {
                if (list != null)
                {
                    Womans = new ObservableCollection<WomanView>(list);
                }
                this.Cursor = Cursors.Arrow;
                goButton.IsEnabled = true;
                if (success)
                    System.Windows.MessageBox.Show("Загрузка данных завершена!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #region for DiaryAPI
        async Task Auth()
        {

            await diaryAPIClient.AuthSecureAsync(UserName.Trim(), passwordBox.SecurePassword);
            //MessageBox.Show(diaryAPIClient.SID);

        }
        private TimeSpan timeoutBetweenRequests = TimeSpan.FromMilliseconds(1000);
        async private Task<List<WomanView>> downloadUsers(string from, string to, CancellationToken cToken)
        {
            int a = Convert.ToInt32(from, 10);
            int b = Convert.ToInt32(to, 10);
            var minimal = Math.Min(a, b);
            var maximal = Math.Max(a, b);
            return await Task.Run(() =>
            {
                var result = new List<WomanView>();
                try
                {
                    for (int i = maximal; i >= minimal; i--)
                    {
                        try
                        {
                            UserUnit user = diaryAPIClient.UserGet(String.Format("{0}", i));
                            if (!String.IsNullOrEmpty(user.sex))
                                if (user.sex.Equals("Мужской")) continue;
                            if (user.journal.Equals("0"))
                                continue;
                            if (true)//(user.sex.Equals("Женский"))
                            {
                                if (!string.IsNullOrEmpty(user.AgeAsStr))
                                {
                                    int age = Convert.ToInt32(user.AgeAsStr.Substring(0, 2), 10);
                                    if ((age > 27) || (age < 18)) continue;
                                }
                                if (!String.IsNullOrEmpty(user.about))
                                {
                                    var about = user.about.ToLower();
                                    if (about.Contains("замужем")) continue;
                                    if (about.Contains("есть дочка")) continue;
                                    if (about.Contains("растёт дочка")) continue;
                                    if (about.Contains("есть сын")) continue;
                                    if (about.Contains("растёт сын")) continue;
                                    if (about.Contains("есть муж")) continue;
                                }
                                //if (!user.city.Equals("Москва") && !user.region.Equals("Московская область"))
                                //    continue;
                                bool flag = false;
                                if (user.interest == null) continue;
                                foreach (var item in user.interest)
                                {
                                    if (item.Value.ToLower().Contains("программ"))
                                        flag = true;
                                }
                                if (!flag) continue;
                                var journal = diaryAPIClient.JournalGet(user.userid, null);
                                if (journal != null)
                                    if (DateTimeValue(journal.Last_post) < dTime) continue;
                                result.Add(new WomanView(user));
                                System.Diagnostics.Debug.WriteLine(string.Format("Visited: {0}", i));
                            }
                        }
                        catch (DiaryAPIClientException exc)
                        {
                            System.Diagnostics.Debug.WriteLine("-------------------------------");
                            System.Diagnostics.Debug.WriteLine(string.Format("Visited: {0}", i));
                            System.Diagnostics.Debug.WriteLine(exc.ToString());
                            System.Diagnostics.Debug.WriteLine("-------------------------------");
                            ;//MessageBox.Show(String.Format("{0}", i) + exc.Message);
                        }
                        catch (System.Net.WebException exc)
                        {
                            System.Diagnostics.Debug.WriteLine("-------------------------------");
                            System.Diagnostics.Debug.WriteLine(string.Format("Visited: {0}", i));
                            System.Diagnostics.Debug.WriteLine(exc.ToString());
                            System.Diagnostics.Debug.WriteLine("-------------------------------");
                        }
                        System.Threading.Thread.Sleep(this.timeoutBetweenRequests);
                        cToken.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception ex)
                { ;}
                return result;
                //In theoretical, you may pass "await" keyword and see the result:
            }, cToken);
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

        private void recalcButton_Click(object sender, RoutedEventArgs e)
        {
            int a = Convert.ToInt32(FromUser, 10);
            if (string.IsNullOrEmpty(ToUser))
            {
                ToUser = String.Format("{0}", a - 5000);
                return;
            }
            int b = Convert.ToInt32(ToUser, 10);
            var minimal = Math.Min(a, b);
            var maximal = Math.Max(a, b);
            FromUser = String.Format("{0}", minimal);
            ToUser = String.Format("{0}", minimal - 5000);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            bool success = true;
            this.Cursor = Cursors.Wait;
            List<WomanView> list = null;

            cancelSource = new CancellationTokenSource();
            try
            {
                await Auth();
                list = await downloadUsersFrom("57517", cancelSource.Token);
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
            catch (DiaryAPI.DiaryAPIClientException ex)
            {
                success = false;
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //catch (Exception ex)
            //{
            //    success = false;
            //    System.Windows.MessageBox.Show(ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            finally
            {
                if (list != null)
                {
                    Womans = new ObservableCollection<WomanView>(list);
                }
                this.Cursor = Cursors.Arrow;
                goButton.IsEnabled = true;
                if (success)
                    System.Windows.MessageBox.Show("Загрузка данных завершена!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        async private Task<List<WomanView>> downloadUsersFrom(string from, CancellationToken cToken)
        {

            return await Task.Run(() =>
            {
                var result = new List<WomanView>();
                try
                {
                    UserUnit muser = diaryAPIClient.UserGet(from);
                    Debug.WriteLine(muser.username);
                    foreach (var pair in muser.Readers2)
                    {
                        Debug.WriteLine(pair.Key);
                        try
                        {
                            UserUnit user = diaryAPIClient.UserGet(String.Format("{0}", pair.Key));

                            if (!String.IsNullOrEmpty(user.sex))
                                if (user.sex.Equals("Мужской")) continue;
                            if (user.journal.Equals("0"))
                                continue;
                            if (true)//(user.sex.Equals("Женский"))
                            {
                                if (!string.IsNullOrEmpty(user.AgeAsStr))
                                {
                                    int age = Convert.ToInt32(user.AgeAsStr.Substring(0, 2), 10);
                                    if ((age > 27) || (age < 18)) continue;
                                }
                                if (!String.IsNullOrEmpty(user.about))
                                {
                                    var about = user.about.ToLower();
                                    if (about.Contains("замужем")) continue;
                                    if (about.Contains("есть дочка")) continue;
                                    if (about.Contains("растёт дочка")) continue;
                                    if (about.Contains("есть сын")) continue;
                                    if (about.Contains("растёт сын")) continue;
                                    if (about.Contains("есть муж")) continue;
                                }
                                //if (!user.city.Equals("Москва") && !user.region.Equals("Московская область"))
                                //    continue;
                                bool flag = false;
                                if (user.interest == null) continue;
                                foreach (var item in user.interest)
                                {
                                    if (item.Value.ToLower().Contains("программ"))
                                        flag = true;
                                }
                                if (!flag) continue;
                                var journal = diaryAPIClient.JournalGet(user.userid, null);
                                if (journal != null)
                                    if (DateTimeValue(journal.Last_post) < dTime) continue;
                                result.Add(new WomanView(user));
                            }
                        }
                        catch (DiaryAPIClientException exc)
                        {
                            Debug.WriteLine(String.Format("{0}", pair.Key) + exc.Message);
                        }
                        catch (System.Net.WebException exc)
                        {
                            Debug.WriteLine(exc.ToString());
                        }
                        Debug.WriteLine(String.Format("Visited: {0}", pair.Key));
                        System.Threading.Thread.Sleep(this.timeoutBetweenRequests);
                        cToken.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception ex)
                { ;}
                return result;
                //In theoretical, you may pass "await" keyword and see the result:
            }, cToken);
        }

        private DateTime dTime = new DateTime(2013, 1, 1);
        public static DateTime DateTimeValue(string date)
        {
            return ConvertFromUnixTimestamp(Convert.ToDouble(date));
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
