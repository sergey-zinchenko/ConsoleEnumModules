﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using ProcessAccessFlags = ConsoleEnumModules.WinApiWrapper.ProcessAccessFlags;

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
                var arrayBytesSize = (uint)(hModules.Length * Marshal.SizeOf(typeof(IntPtr)));
                if (!WinApiWrapper.EnumProcessModules(hProcess, pModules, arrayBytesSize, out var cbNeeded))
                {
                    var error = Marshal.GetLastWin32Error();
                    Console.WriteLine("EnumProcessModules Error: " + error);
                    Environment.Exit(1);
                }
                var modulesCopied = (uint)(cbNeeded / Marshal.SizeOf(typeof(IntPtr)));
                for (var i = 0; i < modulesCopied; i++)
                {
                    var hModule = hModules[i];
                    try
                    {
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
                    finally
                    {
                        WinApiWrapper.CloseHandle(hModule);
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        static void Main()
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
                var hProcess = WinApiWrapper.OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VmRead, false, pid);
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
    }
}