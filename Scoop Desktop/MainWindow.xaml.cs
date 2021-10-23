using Scoop_Desktop.Interfaces;
using Scoop_Desktop.Pages;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Configuration;

namespace Scoop_Desktop
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            if (Instance is null)
                Instance = this;

        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var header = (NavView.SelectedItem as ModernWpf.Controls.NavigationViewItem).Content;

            if (args.IsSettingsSelected)
            {
                if (BottomBar.Margin.Bottom == 0)
                {
                    var sb = BottomBar.FindResource("HideCommandBar") as BeginStoryboard;
                    sb.Storyboard.Begin();
                    sb.Storyboard.Completed += (obj, args) => BottomBar.Visibility = Visibility.Collapsed;
                }

                ContentFrame.Navigate(new Settings());
            }
            else
            {
                if (BottomBar.Margin.Bottom < 0)
                {
                    var sb = BottomBar.FindResource("ShowCommandBar") as BeginStoryboard;
                    sb.Storyboard.Begin();
                    BottomBar.Visibility = Visibility.Visible;
                }

                switch (header)
                {
                    case "List":
                        ContentFrame.Navigate(ScoopList.Instance);
                        break;
                    case "Status":
                        ContentFrame.Navigate(ScoopUpdate.Instance);
                        break;
                    case "Bucket":
                        ContentFrame.Navigate(ScoopBucket.Instance);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load app.config
            bool darkmode = bool.Parse(ConfigurationManager.AppSettings["DarkMode"]);
            Settings.SetTheme(darkmode, false);
        }

        #region Public Methods

        public void ToggleProgressRing(bool value)
        {
            MyProgressRing.IsActive = value;
        }

        public async Task RunTaskWithRingAsync(Task task)
        {
            MyProgressRing.IsActive = true;
            await task;
            MyProgressRing.IsActive = false;
        }

        #endregion

        #region App Buttons

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await RunTaskWithRingAsync((ContentFrame.Content as IPage).Update());
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ModernWpf.Controls.ContentDialog
            {
                Title = "Scoop Update",
                DefaultButton = ModernWpf.Controls.ContentDialogButton.Close,
                Content = ""
            };
            dialog.Loaded += async (obj, args) =>
            {
                await ScoopHelper.UpdateAllAsync((obj, args) =>
                {
                    var text = args.Data?.ToString().Trim();
                    if (string.IsNullOrEmpty(text))
                        return;
                    if (text.StartsWith("Updating"))
                    {
                        dialog.Dispatcher.Invoke(() =>
                        {
                            if (dialog.Content.ToString().Length > 0)
                                dialog.Content += "\n";
                            dialog.Content += text;
                        });
                    }
                });
                dialog.Content += "\nScoop was updated successfully!";
                dialog.CloseButtonText = "Done";
            };
            await dialog.ShowAsync();
        }

        private async void Cache_Click(object sender, RoutedEventArgs e)
        {
            ToggleProgressRing(true);
            var res = await ScoopHelper.ScoopCheckCacheAsync();
            ToggleProgressRing(false);
            if (string.IsNullOrEmpty(res))
            {
                await ContentDialogHelper.Close("No cache file found.");
                return;
            }
            var opt = await ContentDialogHelper.YesNo(res, "Cache", yesText: "Remove All", noText: "Close");
            if (opt == ModernWpf.Controls.ContentDialogResult.Primary)
            {
                await RunTaskWithRingAsync(ScoopHelper.ScoopRemoveCacheAsync());
                await ContentDialogHelper.Close("All caches are removed.");
            }
        }

        #endregion

    }
}
