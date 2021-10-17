using ModernWpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scoop_Desktop.Pages
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (ThemeManager.GetActualTheme(this) == ElementTheme.Light)
            {
                ThemeManager.SetRequestedTheme(this, ElementTheme.Dark);
            }
            else
            {
                ThemeManager.SetRequestedTheme(this, ElementTheme.Light);
            }
        }

        private void ToggleAria2_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ModernWpf.Controls.ToggleSwitch;

            CmdHelper.RunPowershellCommand($"scoop config aria2-enabled {toggle.IsOn}");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

            var aria2_enabled = await CmdHelper.RunPowershellCommandAsync("scoop config aria2-enabled");
            useAria2Toggle.IsOn = aria2_enabled.Contains("True");
            useAria2Toggle.IsEnabled = true;

            var proxy = await CmdHelper.RunPowershellCommandAsync("scoop config proxy");
            if (!proxy.Contains("is not set"))
            {
                ProxyTextBox.Text = proxy.TrimEnd('\r', '\n');
            }

            useAria2Toggle.Toggled += ToggleAria2_Toggled;
            this.Visibility = Visibility.Visible;
        }

        private async void SetProxy_Click(object sender, RoutedEventArgs e)
        {
            var proxy = ProxyTextBox.Text;

            if (string.IsNullOrEmpty(proxy))
            {
                CmdHelper.RunPowershellCommand($"scoop config rm proxy");
            }
            else if (!Regex.IsMatch(proxy, @"(?:\d{1,3}(?(?=:)|\.)){4}:\d{1,5}"))
            {
                await new ModernWpf.Controls.ContentDialog
                {
                    Title = "Proxy",
                    Content = "Invalid proxy value.",
                    CloseButtonText = "Close",
                    DefaultButton = ModernWpf.Controls.ContentDialogButton.Close
                }.ShowAsync();
            }
            else
            {
                CmdHelper.RunPowershellCommand($"scoop config proxy {proxy}");
            }
        }
    }
}
