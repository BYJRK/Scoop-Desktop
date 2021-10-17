using Scoop_Desktop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ModernWpf;
using System.Threading.Tasks;
using ModernWpf.Controls;

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
                var info = await CmdHelper.RunPowershellCommandAsync($"scoop info {app.Name}");
                var dialog = new ContentDialog
                {
                    Title = app.Name,
                    Content = info,
                    CloseButtonText = "Close",
                    DefaultButton = ContentDialogButton.Close
                };

                await dialog.ShowAsync();

            }
            else if (header == "Uninstall")
            {
                var result = await new ContentDialog
                {
                    Title = "Uninstall",
                    Content = $"Are you sure you want to uninstall {app.Name}?",
                    PrimaryButtonText = "OK",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    MyProgressRing.IsActive = true;
                    await CmdHelper.RunPowershellCommandAsync($"scoop uninstall {app.Name}");
                    MyProgressRing.IsActive = false;
                    await new ContentDialog
                    {
                        Title = "Uninstall",
                        Content = $"{app.Name} has been uninstalled.",
                        CloseButtonText = "Close",
                        DefaultButton = ContentDialogButton.Close
                    }.ShowAsync();
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
            var res = await CmdHelper.RunPowershellCommandAsync("scoop list");
            var lines = res.ToTrimmedLines();
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
    }
}
