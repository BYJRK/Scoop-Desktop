using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Scoop_Desktop
{
    public static class CmdHelper
    {
        public static string RunPowershellCommand(string arguments)
        {
            var p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "powershell.exe";
            p.StartInfo.Arguments = arguments;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            return p.StandardOutput.ReadToEnd().Trim('\r', '\n');
        }

        public static async Task<string> RunPowershellCommandAsync(string arguments)
        {
            return await Task.Run(() =>
            {
                var p = new Process();
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "powershell.exe";
                p.StartInfo.Arguments = arguments;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                return p.StandardOutput.ReadToEnd().Trim('\r', '\n');
            });
        }

        /// <summary>
        /// 将控制台输出的内容转为逐行且去掉首尾空格的字符串数组
        /// </summary>
        public static string[] ToTrimmedLines(this string input)
        {
            return Regex
                .Split(input, @"\r?\n")
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToArray();
        }
    }
}
