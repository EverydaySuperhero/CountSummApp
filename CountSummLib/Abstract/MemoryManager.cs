using CountSummLib.Interfaces;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.Abstract
{
    public abstract class MemoryManager
    {
        public static int baseBlockSize = 10000000;

        public int GetBlockSize(long fileLength)
        {
            int parallelCount = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }.MaxDegreeOfParallelism;
            var AvailablePhysicalMemory = (long)((int)new ComputerInfo().AvailablePhysicalMemory / parallelCount);


            if (AvailablePhysicalMemory > baseBlockSize)// если выделяемая память меньше макс размера пакета, вернуть макс количество памяти
            {
                if (fileLength > AvailablePhysicalMemory)//если размер файла меньше выделяемой памяти, пусть размер пакета = файлу, иначе вернуть макс занчение пакета 
                    return baseBlockSize;
                else
                    return (int)fileLength;
            }
            return (int)AvailablePhysicalMemory;
        }
    }
}
