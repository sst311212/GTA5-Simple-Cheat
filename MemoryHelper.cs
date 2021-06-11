using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GTA5Hack
{
    class MemoryHelper
    {
        private Process openProcess { get; set; }

        public MemoryHelper(string ProcessName)
        {
            openProcess = Process.GetProcessesByName(ProcessName).FirstOrDefault();
        }

        public void aobScanModule(out IntPtr labelName, string moduleName, string aobStr)
        {
            try
            {
                if (openProcess == null)
                    throw new Exception();
                ProcessModule m = openProcess.Modules.Cast<ProcessModule>().Where((i) => { return i.ModuleName.Contains(moduleName); }).FirstOrDefault();

                byte[] buff = new byte[m.ModuleMemorySize];
                MyReadProcessMemory(m.BaseAddress, buff, buff.Length);

                int nOffset = aobScanBuff(buff, aobStr);
                if (nOffset == -1)
                    throw new Exception();
                labelName = m.BaseAddress + nOffset;
            }
            catch
            {
                labelName = IntPtr.Zero;
            }
        }

        public T GetValue<T>(IntPtr address)
        {
            try
            {
                if (address == IntPtr.Zero)
                    throw new Exception();
                byte[] buff = new byte[Marshal.SizeOf(default(T))];
                if (MyReadProcessMemory(address, buff, buff.Length) == false)
                    throw new Exception();
                if (typeof(T) == typeof(int))
                    return (T)Convert.ChangeType(BitConverter.ToInt32(buff, 0), typeof(T));
                if (typeof(T) == typeof(float))
                    return (T)Convert.ChangeType(BitConverter.ToSingle(buff, 0), typeof(T));
                if (typeof(T) == typeof(IntPtr))
                    return (T)Convert.ChangeType(Marshal.ReadIntPtr(buff, 0), typeof(T));
                throw new NotImplementedException();
            }
            catch
            {
                return default(T);
            }
        }

        public void SetValue<T>(IntPtr address, T value)
        {
            try
            {
                if (address == IntPtr.Zero)
                    throw new Exception();
                IntPtr buffPtr = Marshal.AllocHGlobal(Marshal.SizeOf(value));
                Marshal.StructureToPtr(value, buffPtr, false);
                byte[] buff = new byte[Marshal.SizeOf(value)];
                Marshal.Copy(buffPtr, buff, 0, buff.Length);
                if (MyWriteProcessMemory(address, buff, buff.Length) == false)
                    throw new Exception();
            }
            catch
            {
                return;
            }
        }

        private int aobScanBuff(byte[] buff, string aobStr)
        {
            string[] aobHexStr = aobStr.Split(' ');
            ushort[] aobHexByte = new ushort[aobHexStr.Length];
            for (int i = 0; i < aobHexStr.Length; i++)
            {
                if (aobHexStr[i].Equals("*"))
                    aobHexByte[i] = 0xFF00;
                else
                    aobHexByte[i] = Convert.ToByte(aobHexStr[i], 16);
            }

            for (int i = 0; i < buff.Length; i++)
            {
                bool flag = true;
                for (int j = 0; j < aobHexByte.Length; j++)
                {
                    if (aobHexByte[j] == 0xFF00)
                        continue;
                    if (buff[i + j] != (aobHexByte[j] & 0xFF))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    return i;
            }

            return -1;
        }

        private bool MyReadProcessMemory(IntPtr lpBaseAddress, byte[] lpBuffer, int nSize)
        {
            try
            {
                if (openProcess == null)
                    throw new Exception();
                IntPtr hProcess = OpenProcess(0x38, false, openProcess.Id), byteRead;
                bool flag = ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, out byteRead);
                CloseHandle(hProcess);
                return flag;
            }
            catch
            {
                return false;
            }
        }

        private bool MyWriteProcessMemory(IntPtr lpBaseAddress, byte[] lpBuffer, int nSize)
        {
            try
            {
                if (openProcess == null)
                    throw new Exception();
                IntPtr hProcess = OpenProcess(0x38, false, openProcess.Id), byteWrite;
                bool flag = WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, out byteWrite);
                CloseHandle(hProcess);
                return flag;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("KERNEL32")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("KERNEL32")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("KERNEL32")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("KERNEL32")]
        static extern bool CloseHandle(IntPtr hObject);
    }
}
