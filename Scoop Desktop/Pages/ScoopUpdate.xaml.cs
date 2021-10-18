using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Scoop_Desktop.Models;
using System.Threading.Tasks;

namespace Scoop_Desktop.Pages
{
    public partial class ScoopUpdate : Page
    {
        public ScoopUpdate()
        {
            InitializeComponent();

            MyListView.ItemsSource = AppList;
        }

        private static DateTime lastUpdate = DateTime.MinValue;

        public static ObservableCollection<AppInfo> AppList = new ObservableCollection<AppInfo>();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshAppStatus();
        }

        private async Task RefreshAppStatus()
        {
            MyProgressRing.IsActive = true;

            var res = await ScoopHelper.GetAppStatusAsync();
            if (!res.Contains("Updates are available for:"))
            {
                // 所有应用都是最新的
                return;
            }

            AppList.Clear();
            foreach (var line in res.ToTrimmedLines().Where(line => line.Contains("->")))
            {
                var split = line.Split(": ");
                AppList.Add(new AppInfo { Name = split[0], Version = split[1] });
            }

            MyProgressRing.IsActive = false;
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            var app = MyListView.SelectedItem as AppInfo;
            if (app is null)
                return;
            switch (item.Header.ToString())
            {
                case "Update":
                    var option = await ContentDialogHelper.YesNo($"Are you sure you want to update {app.Name}?\n({app.Version})", $"Update");
                    if (option == ModernWpf.Controls.ContentDialogResult.Primary)
                    {
                        MyProgressRing.IsActive = true;
                        var res = await ScoopHelper.UpdateAppAsync(app.Name);
                        MyProgressRing.IsActive = false;
                        await ContentDialogHelper.Close(res ? "Update successfully." : "Update failed.");
                        await RefreshAppStatus();
                    }
                    break;
            }
        }
    }
}
