using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scoop_Desktop
{
    class ScoopHelper
    {
        public static readonly string ScoopRootDir;
        public static readonly string ScoopBucketDir;

        static ScoopHelper()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            ScoopRootDir = Path.Combine(home, "scoop");
            ScoopBucketDir = Path.Combine(ScoopRootDir, "buckets");
        }

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

        public static async Task UpdateAppAsync(string appName, DataReceivedEventHandler callback = null)
        {
            await CmdHelper.RunPowershellCommandAsync($"scoop update {appName}", callback);
        }

        public static async Task UpdateAllAsync(DataReceivedEventHandler callback = null)
        {
            await CmdHelper.RunPowershellCommandAsync($"scoop update", callback);
        }

        public static async Task<string> InstallAppAsync(string appName)
        {
            return await CmdHelper.RunPowershellCommandAsync($"scoop install {appName}");
        }

        public static async Task<string> ScoopCheckCacheAsync()
        {
            var res = await CmdHelper.RunPowershellCommandAsync("scoop cache");

            var lines = res.Trim().ToTrimmedLines();

            if (lines.Length == 1)
                return null;

            var text = string
                .Join("\n", lines
                .SkipLast(1)
                .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Where(s => s.Length == 5)
                .Where(s => s[1] != "KB")
                .Select(s => $"{s[0]} {s[1]} {s[2]}")
                );

            text = "(caches less than 1 MB are not shown)\n\n" + text;
            text += "\n\n" + lines[^1];

            return text;
        }

        public static async Task ScoopRemoveCacheAsync()
        {
            await CmdHelper.RunPowershellCommandAsync("scoop cache rm *");
        }
    }
}
