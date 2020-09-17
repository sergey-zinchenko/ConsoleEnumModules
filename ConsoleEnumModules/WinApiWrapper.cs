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
        internal static extern bool EnumProcesses(
            [In, Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)]
            uint[] processIds,
            uint arraySizeBytes,
            out uint bytesCopied);

        [DllImport(Kernel, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
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
            out uint lPcbNeeded);

        [DllImport(PsApi, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern uint GetModuleBaseName(IntPtr hProcess,
            IntPtr hModule,
            [Out] StringBuilder lpBaseName,
            uint nSize);
        
        [DllImport(Kernel, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        internal static extern IntPtr CreateToolhelp32Snapshot(
            [In, MarshalAs(UnmanagedType.U4)] SnapshotFlags dwFlags,
            [In, MarshalAs(UnmanagedType.U4)] uint th32ProcessId
        );
        
        [DllImport(Kernel, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool Process32First([In]IntPtr hSnapshot, ref ProcessEntry32 lppe);

        [DllImport(Kernel, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool Process32Next([In]IntPtr hSnapshot, ref ProcessEntry32 lppe);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct ProcessEntry32
        {
            private const int MaxPath = 260;
            public UInt32 dwSize;
            public UInt32 cntUsage;
            public UInt32 th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public UInt32 th32ModuleID;
            public UInt32 cntThreads;
            public UInt32 th32ParentProcessID;
            public UInt32 pcPriClassBase;
            public UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
            public string szExeFile;
        }
        
        [Flags]
        internal enum ProcessAccessFlags : uint
        {
            VmRead = 0x00000010,
            QueryInformation = 0x00000400,
        }
        
        [Flags]
        internal enum SnapshotFlags : uint
        {
            Process = 0x00000002,
            Module = 0x00000008,
            Module32 = 0x00000010
        }
    }
}