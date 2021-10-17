using Scoop_Desktop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ModernWpf;
using System.Threading.Tasks;

namespace Scoop_Desktop.Pages
{
    public partial class ScoopList : Page
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
                MessageBox.Show(await CmdHelper.RunPowershellCommandAsync($"scoop info {app.Name}"));
            }
            else if (header == "Uninstall")
            {
                var result = MessageBox.Show($"Are you sure you want to uninstall {app.Name}?", null, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    MyProgressRing.IsActive = true;
                    await CmdHelper.RunPowershellCommandAsync($"scoop uninstall {app.Name}");
                    MessageBox.Show($"{app.Name} has been uninstalled.");
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
            var res = await CmdHelper.RunPowershellCommandAsync("scoop list");
            var lines = res.ToTrimmedLines();
            AppList.Clear();
            foreach (var line in lines.Skip(2))
            {
                AppList.Add(new AppInfo(line));
            }
        }
    }
}
