using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ScoopDesktop.Models;
using System.Threading.Tasks;
using ScoopDesktop.Interfaces;
using System.Windows.Data;

namespace ScoopDesktop.Pages
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
                // all apps are in the newest version
                return;
            }

            AppList.Clear();
            foreach (var line in res.ToTrimmedLines().Where(line => line.Contains("->")))
            {
                var split = line.Split(": ");
                var name = split[0];
                var newVersion = split[1].Split(" -> ")[1];
                AppList.Add(new AppInfo { Name = name, Version = split[1] });
                ScoopList.AppList.First(app => app.Name == name).NewVersion = newVersion;
            }
            ScoopList.Instance.MyListView.Items.Refresh();            

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
                    await UpdateAppAsync(app);
                    break;
            }
        }

        private async Task UpdateAppAsync(AppInfo app)
        {
            var option = await ContentDialogHelper.YesNo($"Are you sure you want to update {app.Name}?\n({app.Version})",
                "Update",
                defaultButton: ModernWpf.Controls.ContentDialogButton.Close);

            if (option != ModernWpf.Controls.ContentDialogResult.Primary)
                return;

            bool succeed = false;

            var dialog = new ModernWpf.Controls.ContentDialog
            {
                Title = "Scoop Update",
                DefaultButton = ModernWpf.Controls.ContentDialogButton.Close,
                Content = ""
            };
            dialog.Loaded += async (obj, args) =>
            {
                await ScoopHelper.UpdateAppAsync(app.Name, (obj, args) =>
                {
                    var usefulLines = new string[]
                    {
                        "Updating", "Downloading", "Uninstalling", "Installing"
                    };
                    var text = args.Data?.ToString().Trim();

                    if (string.IsNullOrEmpty(text))
                        return;

                    if (text.EndsWith("was installed successfully!"))
                    {
                        succeed = true;
                        return;
                    }
                    else if (usefulLines.Any(word => text.StartsWith(word)))
                    {
                        dialog.Dispatcher.Invoke(() =>
                        {
                            dialog.Content += text + "\n";
                        });
                    }
                    else if (text.ToLower().Contains("error"))
                    {
                        dialog.Dispatcher.Invoke(() =>
                        {
                            dialog.Content += text;
                        });
                        return;
                    }
                });

                if (succeed)
                {
                    dialog.Content += $"{app.Name} was updated successfully!";
                    dialog.CloseButtonText = "Done";
                }
                else
                {
                    dialog.Content += "Update failed. Try again?";
                    dialog.PrimaryButtonText = "Yes";
                    dialog.CloseButtonText = "No";
                }
            };

            var opt = await dialog.ShowAsync();
            if (opt == ModernWpf.Controls.ContentDialogResult.Primary)
            {
                await UpdateAppAsync(app);
            }
            else if (succeed)
            {
                await RefreshAppStatus();
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
