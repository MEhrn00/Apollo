﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace ExecutePE
{
    internal static unsafe class NativeDeclarations
    {
        internal enum StdHandle
        {
            Stdin = -10,
            Stdout = -11,
            Stderr = -12
        }

        internal const uint PAGE_EXECUTE_READWRITE = 0x40;
        internal const uint PAGE_READWRITE = 0x04;
        internal const uint PAGE_EXECUTE_READ = 0x20;
        internal const uint PAGE_EXECUTE = 0x10;
        internal const uint PAGE_EXECUTE_WRITECOPY = 0x80;
        internal const uint PAGE_NOACCESS = 0x01;
        internal const uint PAGE_READONLY = 0x02;
        internal const uint PAGE_WRITECOPY = 0x08;
        internal const uint MEM_COMMIT = 0x1000;
        internal const uint MEM_RELEASE = 0x00008000;

        internal const uint IMAGE_SCN_MEM_EXECUTE = 0x20000000;
        internal const uint IMAGE_SCN_MEM_READ = 0x40000000;
        internal const uint IMAGE_SCN_MEM_WRITE = 0x80000000;

        public struct IMAGE_BASE_RELOCATION
        {
            internal uint VirtualAddress;
            internal uint SizeOfBlock;

            private IMAGE_BASE_RELOCATION(uint virtualAddress, uint sizeOfBlock)
            {
                VirtualAddress = virtualAddress;
                SizeOfBlock = sizeOfBlock;
            }

            public static IMAGE_BASE_RELOCATION Parse(byte[] b)
            {
                var virtualAddress = BitConverter.ToUInt32(b, 0);
                var sizeOfBlock = BitConverter.ToUInt32(b, 4);
                return new IMAGE_BASE_RELOCATION(virtualAddress, sizeOfBlock);
            }
        }

        internal enum X86BaseRelocationType : byte
        {
            IMAGE_REL_BASED_ABSOLUTE = 0,
            IMAGE_REL_BASED_HIGH = 1,
            IMAGE_REL_BASED_LOW = 2,
            IMAGE_REL_BASED_HIGHLOW = 3,
            IMAGE_REL_BASED_HIGHADJ = 4,
            IMAGE_REL_BASED_DIR64 = 10,
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetStdHandle(StdHandle nStdHandle, IntPtr hHandle);

        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(StdHandle nStdHandle);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES
        {
            internal int nLength;
            internal byte* lpSecurityDescriptor;
            internal int bInheritHandle;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer,
            uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        internal static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe,
            ref SECURITY_ATTRIBUTES lpPipeAttributes, uint nSize);

        [DllImport("ntdll.dll", SetLastError = true)]
        internal static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
            IntPtr processInformation, uint processInformationLength, IntPtr returnLength);

        [DllImport("kernel32")]
        internal static extern IntPtr VirtualAlloc(IntPtr lpStartAddr, uint size, uint flAllocationType,
            uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetCommandLine();

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32")]
        internal static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
            IntPtr param, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        internal static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect,
            out uint lpFlOldProtect);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool AllocConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool AttachConsole(int pid);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool VirtualFree(IntPtr pAddress, uint size, uint freeType);

        [DllImport("kernel32")]
        internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_BASIC_INFORMATION
        {
            internal uint ExitStatus;
            internal IntPtr PebAddress;
            internal UIntPtr AffinityMask;
            internal int BasePriority;
            internal UIntPtr UniqueProcessId;
            internal UIntPtr InheritedFromUniqueProcessId;
        }
    }
}
