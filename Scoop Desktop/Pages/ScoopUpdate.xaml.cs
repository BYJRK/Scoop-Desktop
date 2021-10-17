using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Scoop_Desktop.Models;
using System.Windows.Input;

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
            MyProgressRing.IsActive = true;

            // 如果距离上次更新的日期过去超过 15 分钟，则重新更新
            //if (DateTime.Now - lastUpdate > new TimeSpan(0, 15, 0))
            //{
            //    await CmdHelper.RunPowershellCommandAsync("scoop update");
            //    lastUpdate = DateTime.Now;
            //}

            var res = await CmdHelper.RunPowershellCommandAsync("scoop status");
            if (!res.Contains("Updates are available for:"))
            {
                // 所有应用都是最新的
                return;
            }

            var lines = res
                .ToTrimmedLines()
                .SkipWhile(line => !line.StartsWith("Updates are available for"))
                .Skip(1)
                .SkipLast(1);

            AppList.Clear();
            foreach (var line in lines)
            {
                var split = line.Split(": ");
                AppList.Add(new AppInfo { Name = split[0], Version = split[1] });
            }

            MyProgressRing.IsActive = false;
        }

        private void ListViewItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show($"Are you sure you want to uninstall {((sender as ListViewItem).Content as AppInfo).Name}?");
        }
    }
}
