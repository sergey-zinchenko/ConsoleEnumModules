using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleEnumModules
{
    internal static class WinApiWrapper
    {
        private const string PsApi = "psapi.dll";
        private const string Kernel = "kernel32.dll";
        
        [DllImport(PsApi, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        internal static extern bool EnumProcesses([In, Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] uint[] processIds,
            uint arraySizeBytes,
            out uint bytesCopied);
        
        [DllImport(Kernel,CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            uint dwProcessId);

        [DllImport(Kernel, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport(PsApi, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern uint GetModuleFileNameEx(IntPtr hProcess,
            IntPtr hModule, 
            [Out] StringBuilder lpBaseName, 
            uint nSize);

        [DllImport(PsApi, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumProcessModules(IntPtr hProcess,
            IntPtr lphModule,
            uint cb,
            out uint lpcbNeeded);

        [DllImport(PsApi, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern uint GetModuleBaseName(IntPtr hProcess, 
            IntPtr hModule, 
            [Out] StringBuilder lpBaseName, 
            uint nSize);

        [Flags]
        internal enum ProcessAccessFlags : uint
        {
            VmRead = 0x00000010,
            QueryInformation = 0x00000400,
        }
    }
}