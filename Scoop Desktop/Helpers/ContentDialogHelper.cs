using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ModernWpf;
using ModernWpf.Controls;

namespace Scoop_Desktop
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

        public static async Task<ContentDialogResult> YesNo(object content, string title = "", ContentDialogButton defaultButton = ContentDialogButton.Primary, DialogButtonPair pair = DialogButtonPair.OkCancel)
        {
            string yes;
            string no;
            switch (pair)
            {
                case DialogButtonPair.YesNo:
                    yes = "Yes";
                    no = "No";
                    break;
                case DialogButtonPair.OkCancel:
                    yes = "OK";
                    no = "Cancel";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return await new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = yes,
                CloseButtonText = no,
                DefaultButton = ContentDialogButton.Primary
            }.ShowAsync();
        }

        public enum DialogButtonPair
        {
            YesNo,
            OkCancel,
            ConfirmAbort
        }
    }
}
