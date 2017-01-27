using System;
using System.Runtime.InteropServices;
using Windows.Kernel;

namespace Windows
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            [In] ref StartupInfo lpStartupInfo,
            [Out] out ProcessInformation lpProcessInformation);

        public static ProcessInformation? CreateProcess(string appName, string command) {
            StartupInfo s = new StartupInfo();
            ProcessInformation p = new ProcessInformation();
            bool success = CreateProcess(
                appName,
                command,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0,
                IntPtr.Zero,
                null,
                ref s,
                out p);
            if (success) return p;
            return null;
        }
    }
}
