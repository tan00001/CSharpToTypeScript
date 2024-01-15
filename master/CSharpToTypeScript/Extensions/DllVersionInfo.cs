using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Extensions
{
    internal class DllVersionInfoComparer : IComparer<DllVersionInfo>
    {
        public int Compare(DllVersionInfo? x, DllVersionInfo? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x! > y!)
            {
                return 1;
            }

            if (x! < y!)
            {
                return -1;
            }

            return 0;
        }
    }

    internal class DllVersionInfo
    {
        public static readonly IComparer<DllVersionInfo> Comparer = new DllVersionInfoComparer();

        public string Version { get; set; }

        public int MajorVersion { get; set; }

        public int MinorVersion { get; set; }

        public int Build { get; set; }

        public int Release { get; set; }

        public DllVersionInfo(string version)
        {
            Version = version;

            if (string.IsNullOrEmpty(Version))
            {
                return;
            }

            var versionParts = version.Split('.');

            if (int.TryParse(versionParts[0], out var majorVersion) && majorVersion >= 0)
            {
                MajorVersion = majorVersion;
            }

            if (versionParts.Length > 1 && int.TryParse(versionParts[1], out var minorVersion) && minorVersion >= 0)
            {
                MinorVersion = minorVersion;
            }

            if (versionParts.Length > 2 && int.TryParse(versionParts[2], out var build) && build >= 0)
            {
                Build = build;
            }

            if (versionParts.Length > 3 && int.TryParse(versionParts[3], out var release) && release >= 0)
            {
                Release = release;
            }
        }

        public static bool operator >(DllVersionInfo a, DllVersionInfo b)
        {
            if (a.MajorVersion != b.MajorVersion)
            {
                return a.MajorVersion > b.MajorVersion;
            }

            if (a.MinorVersion != b.MinorVersion)
            {
                return a.MinorVersion > b.MinorVersion;
            }

            if (a.Build != b.Build)
            {
                return a.Build > b.Build;
            }

            return a.Release > b.Release;
        }

        public static bool operator <(DllVersionInfo a, DllVersionInfo b)
        {
            if (a.MajorVersion != b.MajorVersion)
            {
                return a.MajorVersion < b.MajorVersion;
            }

            if (a.MinorVersion != b.MinorVersion)
            {
                return a.MinorVersion < b.MinorVersion;
            }

            if (a.Build != b.Build)
            {
                return a.Build < b.Build;
            }

            return a.Release < b.Release;
        }

        public override string ToString()
        {
            return Version;
        }
    }
}
