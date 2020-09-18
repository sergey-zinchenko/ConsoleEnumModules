using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleEnumModules
{
    internal static class WinApiWrapper
    {
        private const string PsApi = "psapi.dll";
        private const string Kernel = "kernel32.dll";
        private const int MaxPath = 260;
        private const int MaxModuleName32 = 255;
        
        internal static IntPtr InvalidHandleValue = new IntPtr(-1);

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
            uint th32ProcessId
        );
        
        [DllImport(Kernel, CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool Process32First(IntPtr hSnapshot, ref ProcessEntry32 lppe);

        [DllImport(Kernel, CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool Process32Next(IntPtr hSnapshot, ref ProcessEntry32 lppe);
        
        [DllImport(Kernel, CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool Module32First(IntPtr hSnapshot, ref ModuleEntry32 lpme);
        
        [DllImport(Kernel, CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool Module32Next(IntPtr hSnapshot, ref ModuleEntry32 lpme);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct ProcessEntry32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public uint pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
            public string szExeFile;
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct ModuleEntry32
        {
            public uint dwSize;
            public uint th32ModuleID;
            public uint th32ProcessID;
            public uint GlblcntUsage;
            public uint ProccntUsage;
            public IntPtr modBaseAddr;
            public uint modBaseSize;
            public IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxModuleName32 + 1)]
            public string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
            public string szExePath;
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
            Module = 0x00000008
        }
    }
}