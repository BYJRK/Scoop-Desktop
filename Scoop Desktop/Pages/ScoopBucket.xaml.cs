using ScoopDesktop.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ScoopDesktop.Pages
{
    /// <summary>
    /// ScoopBucket.xaml 的交互逻辑
    /// </summary>
    public partial class ScoopBucket : Page, IPage
    {
        private static ScoopBucket instance;
        public static ScoopBucket Instance
        {
            get
            {
                if (instance is null)
                    instance = new ScoopBucket();
                return instance;
            }
        }

        public ScoopBucket()
        {
            InitializeComponent();

            BucketAppListView.ItemsSource = appList;

            var view = CollectionViewSource.GetDefaultView(BucketAppListView.ItemsSource);
            view.Filter = (item) =>
            {
                var searchText = SearchBox.Text;

                if (string.IsNullOrEmpty(searchText))
                    return true;
                return (item as string).Contains(searchText);
            };
        }

        ObservableCollection<string> appList = new ObservableCollection<string>();

        private void BucketListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bucketName = BucketListComboBox.SelectedItem.ToString();
            var apps = Path.Combine(ScoopHelper.ScoopBucketDir, bucketName, "bucket");

            appList.Clear();
            foreach (var file in Directory.GetFiles(apps))
            {
                appList.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        private void AutoSuggestBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            CollectionViewSource.GetDefaultView(BucketAppListView.ItemsSource).Refresh();
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            var header = item?.Header.ToString();
            var appName = BucketAppListView.SelectedItem.ToString();
            switch (header)
            {
                case "Info":
                    MainWindow.Instance.ToggleProgressRing(true);
                    await ScoopHelper.ShowAppInfoAsync(appName, () => MainWindow.Instance.ToggleProgressRing(false));
                    break;
                case "Install":
                    await MainWindow.Instance.RunTaskWithRingAsync(ScoopHelper.InstallAppAsync(appName));
                    await ContentDialogHelper.Close($"{appName} was installed successfully.");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            RefreshBuckets();
        }

        private void RefreshBuckets()
        {
            foreach (var bucket in Directory.GetDirectories(ScoopHelper.ScoopBucketDir))
            {
                BucketListComboBox.Items.Add(Path.GetFileName(bucket));
            }
            BucketListComboBox.SelectedItem = "main";
        }

        public Task Update()
        {
            return Task.Run(() => Application.Current.Dispatcher.Invoke(() => RefreshBuckets()));
        }
    }
}
