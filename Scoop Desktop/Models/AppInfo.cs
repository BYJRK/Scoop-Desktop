using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoopDesktop.Models
{
    public class AppInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Bucket { get; set; }
        public string NewVersion { get; set; }
        public AppInfo(string scoopInfo)
        {
            var split = scoopInfo.Split();
            Name = split[0];
            Version = split[1];
            Bucket = split[2].Trim('[', ']');
        }

        public AppInfo()
        {

        }

        public override string ToString()
        {
            return $"{Name}, {Version}, {Bucket}";
        }
    }
}
