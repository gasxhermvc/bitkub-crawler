using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Utility
{
    public static class AssemblyExtensions
    {
        public static Assembly AssemblyFromName(string assemblyName)
        {
            Assembly assembly = null;
            string assemblySearchString = string.Empty;
            string[] assemblyProcessor = { "", ", processorArchitecture='AMD64'", ", processorArchitecture='x86'" };
            AssemblyInfo aInfo;
            IAssemblyCache ac;
            int hr = -1;
            foreach (string processor in assemblyProcessor)
            {
                try
                {
                    aInfo = new AssemblyInfo();
                    assemblySearchString = assemblyName + processor;
                    aInfo.cchBuf = 1024; // should be fine...
                    aInfo.currentAssemblyPath = new String('\0', aInfo.cchBuf);
                    hr = CreateAssemblyCache(out ac, 0);
                    if (hr >= 0)
                    {
                        hr = ac.QueryAssemblyInfo(0, assemblySearchString, ref aInfo);
                        if (hr < 0)
                            throw new Exception("Assembly not found");
                    }
                    assembly = Assembly.LoadFrom(aInfo.currentAssemblyPath);
                    if (assembly == null) throw new Exception("Assembly not found");
                    return assembly;
                }
                catch { continue; }
            }
            try
            {
                assembly = System.Reflection.Assembly.Load(assemblyName);
                return assembly;
            }
            catch
            {
                throw new Exception("Assembly not found");
            }
        }

        [System.Runtime.InteropServices.ComImport, System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown), System.Runtime.InteropServices.Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
        private interface IAssemblyCache
        {
            void Reserved0();

            [System.Runtime.InteropServices.PreserveSig]
            int QueryAssemblyInfo(int flags, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string assemblyName, ref AssemblyInfo assemblyInfo);
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct AssemblyInfo
        {
            public int cbAssemblyInfo;
            public int assemblyFlags;
            public long assemblySizeInKB;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string currentAssemblyPath;
            public int cchBuf; // size of path buf.
        }

        [System.Runtime.InteropServices.DllImport("fusion.dll")]
        private static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);
    }
}
