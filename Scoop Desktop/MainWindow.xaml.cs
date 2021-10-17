using Scoop_Desktop.Models;
using Scoop_Desktop.Pages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Scoop_Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var header = (NavView.SelectedItem as ModernWpf.Controls.NavigationViewItem).Content;

            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(new Settings());
            }
            else
            {
                switch (header)
                {
                    case "List":
                        ContentFrame.Navigate(new ScoopList());
                        break;
                    case "Update":
                        ContentFrame.Navigate(new ScoopUpdate());
                        break;
                    case "Bucket":
                        ContentFrame.Navigate(new ScoopBucket());
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
