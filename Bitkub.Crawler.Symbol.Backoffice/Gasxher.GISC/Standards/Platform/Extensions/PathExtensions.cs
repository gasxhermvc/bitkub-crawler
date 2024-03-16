using GasxherGIS.Standards.Platform.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Platform.Extensions
{
    public static class PathExtensions
    {
        public static string FormatPath(this IPlatformRuntime platform, string pathFile)
        {
            //=>Windows Platform Support
            if (platform.IsWindows())
            {
                if (pathFile.IsVirtualPath())
                {
                    return pathFile.FormatVirtualPathPretty();
                }

                if (pathFile.IsWindowsPath())
                {
                    return pathFile.AutoGeneratePath()
                        .FormatDirectorylPathPretty()
                        .DoBuildBeforePath();
                }
            }


            if (platform.IsOSX() || platform.IsLinux())
            {
                if (pathFile.IsVirtualPath())
                {
                    return pathFile.FormatVirtualPathPretty();
                }

                if (pathFile.IsUnixOrLinuxPath())
                {
                    return pathFile.AutoGeneratePath()
                        .FormatDirectorylPathPretty()
                        .DoBuildBeforePath();
                }
            }

            //=>Other Platform
            return pathFile;
        }

        public static string CombinePath(this IPlatformRuntime platform, params string[] paths)
        {
            return platform.FormatPath(System.IO.Path.Combine(paths));
        }

        public static string GetDirectoryName(this IPlatformRuntime platform, string pathFile)
        {
            return platform.FormatPath(System.IO.Path.GetDirectoryName(pathFile));
        }

        #region Check Windows Path
        /*D:/.... True
            C:/... True
            C:/... True
            C:/Temp True
            D: True
            C: True
            C:/a/d/cc/a.txt True
            C:/csdd True
            C:/dsd.cs True
            C:/dsd.cs True
            C:/dsd/ccc.cs True
            C:/dsd/ccc.cs True
            C:/dsd/ssss/ccc.cs True
            C:/dsd/ssss/dd/ccc.cs True
            ../a True
            ../a True
            \\a True
            a True
            c/a/b True
            \\c True
            \\c False
            \\c False
            \\c True*/
        internal static bool IsWindowsPath(this string pathFile)
        {
            /*D://.... False
              C://... False
              C:\... True
              C:\Temp True
              D:/ False
              C:\ True
              \a False
              a False
              c\a\b\ False
              \\\\c False
              \\c False
              \\c False
              /c False*/
            if (Regex.IsMatch(pathFile, @"^(\w+)\:\\.*$", RegexOptions.IgnoreCase))
            {
                return true;
            }
            /**
             ../a True
            ..\\a True
            */
            if (Regex.IsMatch(pathFile, @"^(\.\.\/|\.\.\\|\.\.\\\\)(\w+).*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /*D://.... True
              C://... True
              C:\... False
              C:\Temp False
              D:/ False
              C:\ False
              \a False
              a False
              c\a\b\ False
              \\\\c False
              \\c False
              \\c False
              /c False*/
            if (Regex.IsMatch(pathFile, @"^(\w+)\:\//.*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /* D://.... True
                C://... True
                C:\... True
                C:\Temp True
                D:/ True
                C:\ True
                \a False
                a True
                c\a\b\ True
                \\\\c False
                \\c False
                \\c False
                /c False*/
            if (Regex.IsMatch(pathFile, @"^(\w+).*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /* D://.... False
                C://... False
                C:\... False
                C:\Temp False
                D:/ False
                C:\ False
                \a True
                a False
                c\a\b\ False
                \\\\c False
                \\c False
                \\c False
                /c True*/
            if (Regex.IsMatch(pathFile, @"^(\/|\\)(\w+).*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            return true;
        }
        #endregion

        #region Check Unix-Linux Path
        /*D:/.... False
            C:/... False
            C:/... False
            C:/Temp False
            D: True
            C: True
            C:/a/d/cc/a.txt False
            C:/csdd False
            C:/dsd.cs False
            C:/dsd.cs False
            C:/dsd/ccc.cs False
            C:/dsd/ccc.cs False
            C:/dsd/ssss/ccc.cs False
            C:/dsd/ssss/dd/ccc.cs False
            ../a True
            ../a True
            \\a True
            a True
            c/a/b True
            \\c True*/
        internal static bool IsUnixOrLinuxPath(this string pathFile)
        {
            /**
             ../a True
            ..\\a True
            */
            if (Regex.IsMatch(pathFile, @"^(\.\.\/|\.\.\\|\.\.\\\\)(\w+).*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /* D://.... True
                C://... True
                C:\... True
                C:\Temp True
                D:/ True
                C:\ True
                \a False
                a True
                c\a\b\ True
                \\\\c False
                \\c False
                \\c False
                /c False*/
            if (Regex.IsMatch(pathFile, @"^(\w+).*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /* D://.... False
                C://... False
                C:\... False
                C:\Temp False
                D:/ False
                C:\ False
                \a True
                a False
                c\a\b\ False
                \\\\c False
                \\c False
                \\c False
                /c True*/
            if (Regex.IsMatch(pathFile, @"^(\/|\\)(\w+).*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            return true;
        }
        #endregion

        #region Check Virtual Path
        /*D://.... False
            C://... False
            C:\... False
            C:\Temp False
            D:/ False
            C:\ False
            \a False
            a False
            c\a\b\ False
            \\\\c False
            \\c False
            \\c False
            /c False
            \\192.168.32.1 True
            \\192.168.32.1/ True
            \\192.168.32.1/1233/233 True
            \\192.168.32.1\1233\233 True
            //192.168.32.1 True
            //192.168.32.1/ True
            //192.168.32.1\ True
            //192.168.32.1\a True
            Bcbgis.dpt.go.th True
            Bcbgis.dpt.go.th True
            Bcbgis.dpt.go.th/ True
            Bcbgis.dpt.go.th/a True
            Bcbgis.dpt.go.th/ True
            \Bcbgis.dpt.go.th/ False
            \\Bcbgis.dpt.go.th/ False
            \\Bcbgis.dpt.go.th False*/
        internal static bool IsVirtualPath(this string pathFile)
        {
            //=>IP
            /*\\192.168.32.1 True
            \\192.168.32.1/ True
            \\192.168.32.1/1233/233 True
            \\192.168.32.1\1233\233 True
            //192.168.32.1 True
            //192.168.32.1/ True
            //192.168.32.1\ True
            //192.168.32.1\a True*/
            if (Regex.IsMatch(pathFile, @"^(\\\\|\/\/)\b(?:\d{1,3}\.){3}\d{1,3}\b.*", RegexOptions.IgnoreCase))
            {
                return true;
            }

            //=>Domain
            /* Bcbgis.dpt.go.th True
            Bcbgis.dpt.go.th True
            Bcbgis.dpt.go.th/ True
            Bcbgis.dpt.go.th/a True
            Bcbgis.dpt.go.th/ True*/
            if (Regex.IsMatch(pathFile, @"^[0-9\p{L}][0-9\p{L}-\.]{1,61}[0-9\p{L}]\.[0-9\p{L}][\p{L}-]*[0-9\p{L}]+.*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            return false;

        }
        #endregion

        #region Format path pretty
        /*C:/Temp False
            D: False
            C: False
            \\a False
            a False
            c/a/b False
            \\c False
            \\c False
            \\c False
            \\c False
            \\192.168.32.1 True
            \\192.168.32.1 True
            \\192.168.32.1/1233/233 True
            \\192.168.32.1/1233/233 True
            \\192.168.32.1 True
            \\192.168.32.1 True
            \\192.168.32.1 True
            \\192.168.32.1/a True
            Bcbgis.dpt.go.th True
            Bcbgis.dpt.go.th True
            Bcbgis.dpt.go.th True
            Bcbgis.dpt.go.th/a True
            Bcbgis.dpt.go.th True
            \\Bcbgis.dpt.go.th False
            \\Bcbgis.dpt.go.th False
            \\Bcbgis.dpt.go.th False*/
        //=>Format ให้อยู่ในรูปแบบเดียวกันทั้งหมด
        public static string FormatVirtualPathPretty(this string pathFile)
        {
            var formatter = pathFile.Replace("\\", "/").Replace("//", "/").TrimEnd('/');
            return formatter.StartsWith("/") ? $"\\\\{formatter.TrimStart('/')}" : formatter;
        }

        /*D:/.... True
            C:/... True
            C:/... True
            C:/Temp True
            D: True
            C: True
            C:/a/d/cc/a.txt True
            C:/csdd True
            C:/dsd.cs True
            C:/dsd.cs True
            C:/dsd/ccc.cs True
            C:/dsd/ccc.cs True
            C:/dsd/ssss/ccc.cs True
            C:/dsd/ssss/dd/ccc.cs True
            \\a True
            a True
            c/a/b True
            \\c False
            \\c False
            \\c False
            \\c True*/
        public static string FormatDirectorylPathPretty(this string pathFile)
        {
            //=>Support Windows: ใช่
            //=>Support MacOS: ใช่
            //=>Support Linux: ยังไม่ได้ทดสอบ
            var isVirtualPath = pathFile.StartsWith("//") || pathFile.StartsWith("\\\\");
            var formatter = pathFile.Replace("\\", "/").Replace("//", "/").TrimEnd('/');
            return isVirtualPath ? $"\\\\{formatter.TrimStart('/').TrimStart('/')}" : formatter;
        }

        //=>Convert: D:/a/../b/a.txt -> D:/b/a.txt

        //=>Input: /d/s/d/../../a/a.txt 1,2,
        public static string DoBuildBeforePath(this string pathFile)
        {
            string[] splitingPath = pathFile.Split('/');


            List<string> paths = new List<string>();

            int index = 1;

            foreach(var path in splitingPath)
            {
                if(path == string.Empty)
                {
                    paths.Add(path);
                }
                else if(path == "..")
                {
                    if((paths.Count - (index - 1)) < 0)
                    {
                        throw new InvalidOperationException($"Path {pathFile} Invalid Format.");
                    }
                    paths.RemoveAt(paths.Count - 1);
                    index--;
                    continue;
                }
                else
                {
                    paths.Add(path);
                }

                index++;
            }

            return string.Join('/', paths);
        }

        //=>สำหรับรายการที่เป็น Directory แบบไม่เต็ม Path เท่านั้น
        //=>จะนำ AppCurrent.BaseDirectory มาเเติมให้เอง
        internal static string AutoGeneratePath(this string pathFile)
        {
            //=>Directory แบบไม่เต็ม
            if (!Regex.IsMatch(pathFile, @"^(\w+)\:(\\|\/).*$", RegexOptions.IgnoreCase))
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('/').Trim('\\'), pathFile);
            }

            return pathFile;
        }
        #endregion
    }
}
