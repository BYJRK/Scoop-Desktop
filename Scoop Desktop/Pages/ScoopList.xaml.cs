using ScoopDesktop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Diagnostics;
using ScoopDesktop.Interfaces;
using System.IO;

namespace ScoopDesktop.Pages
{
    public partial class ScoopList : Page, IPage
    {
        private static ScoopList instance;
        public static ScoopList Instance
        {
            get
            {
                if (instance is null)
                    instance = new ScoopList();
                return instance;
            }
        }

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

            if (header == "Home Page")
            {
                await CmdHelper.RunPowershellCommandAsync($"scoop home {app.Name}");
            }
            else if (header == "Info")
            {
                MainWindow.Instance.ToggleProgressRing(true);
                await ScoopHelper.ShowAppInfoAsync(app.Name, () => MainWindow.Instance.ToggleProgressRing(false));
            }
            else if (header == "Show in Explorer")
            {
                var folderPath = Path.Combine(ScoopHelper.ScoopRootDir, "apps", app.Name);
                if (!Directory.Exists(folderPath))
                {
                    await ContentDialogHelper.Close($"Cannot find {app.Name}'s directory.");
                }
                else
                    Process.Start("explorer.exe", folderPath);
            }
            else if (header == "Uninstall")
            {
                var result = await ContentDialogHelper.YesNo($"Are you sure you want to uninstall {app.Name}?", "Uninstall");
                if (result == ModernWpf.Controls.ContentDialogResult.Primary)
                {
                    await MainWindow.Instance.RunTaskWithRingAsync(CmdHelper.RunPowershellCommandAsync($"scoop uninstall {app.Name}"));
                    await ContentDialogHelper.Close($"{app.Name} has been uninstalled.");
                    await MainWindow.Instance.RunTaskWithRingAsync(RefreshAppList());
                }
            }
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

        private void ListViewHeader_Click(object sender, RoutedEventArgs e)
        {
            var header = e.OriginalSource as GridViewColumnHeader;
            Debug.WriteLine(header.Column.Header);
        }

        public async Task Update()
        {
            await RefreshAppList();
        }

        private async void Page_Initialized(object sender, System.EventArgs e)
        {
            await MainWindow.Instance.RunTaskWithRingAsync(RefreshAppList());
        }
    }
}
