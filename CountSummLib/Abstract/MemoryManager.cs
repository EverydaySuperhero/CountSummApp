using CountSummLib.Interfaces;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.Abstract
{
    public abstract class MemoryManager
    {
        //public const int MemoryMax32;
        //public const long MemoryMax64;
        //public static int MemoryBlockMax32;
        //public static long MemoryBlockMax64;

        public int GetBlockSize(long fileLength)
        {

            try
            {
                int parallelCount = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }.MaxDegreeOfParallelism;
                //var AvailablePhysicalMemory = (long)((long)new ComputerInfo().AvailablePhysicalMemory*0.5f / parallelCount);
                long MemoryMax = Is64Bit() ? (long)int.MaxValue * 2 : int.MaxValue;


                long MemoryBlockMax = (long)((MemoryMax / parallelCount) * 0.7f);
                //if(MemoryBlockMax > int.MaxValue)


                if (MemoryBlockMax > fileLength)
                    return (int)fileLength;

                else
                     if (MemoryBlockMax > int.MaxValue)
                    return int.MaxValue - 2;
                else
                    return (int)MemoryBlockMax;

            }
            catch (Exception)
            {
                return 1024;
            }

        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        private static bool Is64Bit()
        {
            bool retVal;
            IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
            return retVal;
        }

    }
}
