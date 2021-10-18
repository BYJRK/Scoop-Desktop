using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoop_Desktop
{
    class ScoopHelper
    {
        /// <summary>
        /// 显示应用的详细信息（scoop info appname）
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="callbackBeforeDialog">在弹出提示窗口前需要执行的方法</param>
        /// <returns></returns>
        public static async Task ShowAppInfoAsync(string appName, Action callbackBeforeDialog = null)
        {
            var info = await CmdHelper.RunPowershellCommandAsync($"scoop info {appName}");
            
            if (callbackBeforeDialog != null)
                callbackBeforeDialog.Invoke();

            await ContentDialogHelper.Close(info, appName);
        }

        public static async Task<string[]> GetAppListAsync()
        {
            var res = await CmdHelper.RunPowershellCommandAsync("scoop list");
            return res.ToTrimmedLines();
        }

        public static async Task<string> GetAppStatusAsync()
        {
            return await CmdHelper.RunPowershellCommandAsync("scoop status");
        }

        public static async Task<bool> UpdateAppAsync(string appName)
        {
            var res = await CmdHelper.RunPowershellCommandAsync($"scoop update {appName}");
            if (res.ToTrimmedLines().Last().Contains("was installed successfully!"))
                return true;
            return false;
        }
    }
}
