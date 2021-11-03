using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ModernWpf;
using ModernWpf.Controls;

namespace ScoopDesktop
{
    class ContentDialogHelper
    {
        public static async Task<ContentDialogResult> Close(object content, string title = "", string buttonText = "Close")
        {
            return await new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = buttonText,
                DefaultButton = ContentDialogButton.Close
            }.ShowAsync();
        }

        public static async Task<ContentDialogResult> YesNo(object content, string title = "", ContentDialogButton defaultButton = ContentDialogButton.Primary, string yesText = "Yes", string noText = "no")
        {
            return await new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = yesText,
                CloseButtonText = noText,
                DefaultButton = ContentDialogButton.Primary
            }.ShowAsync();
        }
    }
}
