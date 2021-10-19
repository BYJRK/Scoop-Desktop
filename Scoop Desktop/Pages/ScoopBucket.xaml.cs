using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Scoop_Desktop.Pages
{
    /// <summary>
    /// ScoopBucket.xaml 的交互逻辑
    /// </summary>
    public partial class ScoopBucket : Page
    {
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

        private string bucketsDir;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            bucketsDir = Path.Combine(home, "scoop", "buckets");

            foreach (var bucket in Directory.GetDirectories(bucketsDir))
            {
                BucketListComboBox.Items.Add(Path.GetFileName(bucket));
            }
            BucketListComboBox.SelectedItem = "main";
        }

        private void BucketListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bucketName = BucketListComboBox.SelectedItem.ToString();
            var apps = Path.Combine(bucketsDir, bucketName, "bucket");

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
                    MyProgressRing.IsActive = true;
                    await ScoopHelper.ShowAppInfoAsync(appName, () => MyProgressRing.IsActive = false);
                    break;
                case "Install":
                    MyProgressRing.IsActive = true;
                    await ScoopHelper.InstallAppAsync(appName);
                    MyProgressRing.IsActive = false;
                    await ContentDialogHelper.Close($"{appName} was installed successfully.");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
