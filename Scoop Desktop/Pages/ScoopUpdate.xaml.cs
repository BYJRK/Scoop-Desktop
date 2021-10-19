using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Scoop_Desktop.Models;
using System.Threading.Tasks;
using Scoop_Desktop.Interfaces;

namespace Scoop_Desktop.Pages
{
    public partial class ScoopUpdate : Page, IPage
    {
        private static ScoopUpdate instance;
        public static ScoopUpdate Instance
        {
            get
            {
                if (instance is null)
                    instance = new ScoopUpdate();
                return instance;
            }
        }

        public ScoopUpdate()
        {
            InitializeComponent();

            MyListView.ItemsSource = AppList;
        }

        private static DateTime lastUpdate = DateTime.MinValue;

        public static ObservableCollection<AppInfo> AppList = new ObservableCollection<AppInfo>();

        private async Task RefreshAppStatus()
        {
            MainWindow.Instance.ToggleProgressRing(true);

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

            MainWindow.Instance.ToggleProgressRing(false);
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
                        MainWindow.Instance.ToggleProgressRing(true);
                        var res = await ScoopHelper.UpdateAppAsync(app.Name);
                        MainWindow.Instance.ToggleProgressRing(false);

                        if (res.Contains("was installed successfully"))
                        {
                            await ContentDialogHelper.Close($"{app.Name} was updated successfully.");
                            await RefreshAppStatus();
                        }
                        else if (res.Contains("ERROR Application is still running"))
                        {
                            await ContentDialogHelper.Close("Update failed. Application is still running.");
                        }
                        else
                        {
                            await ContentDialogHelper.Close("Update failed.\n" + res);
                        }
                    }
                    break;
            }
        }

        private async void Page_Initialized(object sender, EventArgs e)
        {
            await RefreshAppStatus();
        }

        public Task Update()
        {
            return RefreshAppStatus();
        }
    }
}
