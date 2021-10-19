using Scoop_Desktop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ModernWpf;
using System.Threading.Tasks;
using ModernWpf.Controls;
using System.Diagnostics;

namespace Scoop_Desktop.Pages
{
    public partial class ScoopList : System.Windows.Controls.Page
    {
        public ScoopList()
        {
            InitializeComponent();
            MyListView.ItemsSource = AppList;
        }

        public static ObservableCollection<AppInfo> AppList = new ObservableCollection<AppInfo>();

        private async void MenuItem_RightClick(object sender, RoutedEventArgs e)
        {
            var header = (sender as MenuItem)?.Header.ToString();
            var app = MyListView.SelectedItem as AppInfo;

            if (header == "Home")
            {
                await CmdHelper.RunPowershellCommandAsync($"scoop home {app.Name}");
            }
            else if (header == "Info")
            {
                MyProgressRing.IsActive = true;
                await ScoopHelper.ShowAppInfoAsync(app.Name, () => MyProgressRing.IsActive = false);
            }
            else if (header == "Uninstall")
            {
                var result = await ContentDialogHelper.YesNo($"Are you sure you want to uninstall {app.Name}?", "Uninstall");
                if (result == ContentDialogResult.Primary)
                {
                    MyProgressRing.IsActive = true;
                    await CmdHelper.RunPowershellCommandAsync($"scoop uninstall {app.Name}");
                    MyProgressRing.IsActive = false;
                    await ContentDialogHelper.Close($"{app.Name} has been uninstalled.");
                    MyProgressRing.IsActive = true;
                    await RefreshAppList();
                    MyProgressRing.IsActive = false;
                }
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MyProgressRing.IsActive = true;
            await RefreshAppList();
            MyProgressRing.IsActive = false;
        }

        private async Task RefreshAppList()
        {
            var lines = await ScoopHelper.GetAppListAsync();

            AppList.Clear();
            foreach (var line in lines.Skip(2))
            {
                AppList.Add(new AppInfo(line));
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            MyProgressRing.IsActive = true;
            await RefreshAppList();
            MyProgressRing.IsActive = false;
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListViewHeader_Click(object sender, RoutedEventArgs e)
        {
            var header = e.OriginalSource as GridViewColumnHeader;
            Debug.WriteLine(header.Column.Header);
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            MyProgressRing.IsActive = true;
            await ScoopHelper.UpdateAllAsync();
            await ContentDialogHelper.Close("Scoop has been updated.");
            MyProgressRing.IsActive = false;
        }
    }
}
