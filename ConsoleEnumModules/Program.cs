using System;
using System.Runtime.InteropServices;
using System.Text;
using ProcessAccessFlags = ConsoleEnumModules.WinApiWrapper.ProcessAccessFlags;
using SnapshotFlags = ConsoleEnumModules.WinApiWrapper.SnapshotFlags;
using ProcessEntry = ConsoleEnumModules.WinApiWrapper.ProcessEntry32;

namespace ConsoleEnumModules
{
    static class Program
    {
        private static void WalkProcess(IntPtr hProcess)
        {
            var hModules = new IntPtr[512];
            var gcHandle = GCHandle.Alloc(hModules, GCHandleType.Pinned);
            try
            {
                var pModules = gcHandle.AddrOfPinnedObject();
                var arrayBytesSize = (uint) (hModules.Length * Marshal.SizeOf(typeof(IntPtr)));
                if (!WinApiWrapper.EnumProcessModules(hProcess, pModules, arrayBytesSize, out var cbNeeded))
                {
                    var error = Marshal.GetLastWin32Error();
                    Console.WriteLine("EnumProcessModules Error: " + error);
                    return;
                }

                var modulesCopied = (uint) (cbNeeded / Marshal.SizeOf(typeof(IntPtr)));
                for (var i = 0; i < modulesCopied; i++)
                {
                    var hModule = hModules[i];
                    var strBuilder = new StringBuilder(512);
                    // if (WinApiWrapper.GetModuleFileNameEx(hProcess, hModule, strBuilder,
                    //     (uint) strBuilder.Capacity) == 0)
                    if (WinApiWrapper.GetModuleBaseName(hProcess, hModule, strBuilder,
                        (uint) strBuilder.Capacity) == 0)
                    {
                        var error = Marshal.GetLastWin32Error();
                        Console.WriteLine("GetModuleBaseName Error: " + error);
                        continue;
                    }

                    Console.WriteLine(strBuilder.ToString());
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static void EnumModules()
        {
            var processIds = new uint[512];
            var arrayBytesSize = (uint) (processIds.Length * Marshal.SizeOf(typeof(uint)));

            if (!WinApiWrapper.EnumProcesses(processIds, arrayBytesSize, out var bytesCopied))
            {
                var error = Marshal.GetLastWin32Error();
                Console.WriteLine("EnumProcesses Error: " + error);
                Environment.Exit(1);
            }

            var idsCopied = bytesCopied / Marshal.SizeOf(typeof(uint));

            for (var i = 0; i < idsCopied; i++)
            {
                var pid = processIds[i];
                Console.WriteLine($"/////////////////////// PID = {pid} ///////////////////////");
                var hProcess =
                    WinApiWrapper.OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VmRead, false,
                        pid);
                if (hProcess == IntPtr.Zero)
                {
                    var error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"OpenProcess Error: {error}");
                    continue;
                }

                try
                {
                    WalkProcess(hProcess);
                }
                finally
                {
                    WinApiWrapper.CloseHandle(hProcess);
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static void EnumModules2()
        {
            var snapshot = WinApiWrapper.CreateToolhelp32Snapshot(
                SnapshotFlags.Process, 0);
            if (snapshot == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                Console.WriteLine($"CreateToolhelp32Snapshot Error: {error}");
                return;
            }
            try
            {
                ProcessEntry processEntry = new ProcessEntry();
                processEntry.dwSize = (uint)Marshal.SizeOf(typeof(ProcessEntry));
                if (!WinApiWrapper.Process32First(snapshot, ref processEntry))
                {
                    var error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Process32First Error: {error}");
                    return;
                }
                do
                {
                    Console.WriteLine(processEntry.szExeFile);
                } 
                while (WinApiWrapper.Process32Next(snapshot, ref processEntry));
            }
            finally
            {
                WinApiWrapper.CloseHandle(snapshot);
            }
        }

        static void Main()
        {   
            //EnumModules();
            EnumModules2();
        }
    }
}