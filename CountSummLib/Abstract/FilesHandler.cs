using CountSummLib.Interfaces;
using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.Abstract
{
    public abstract class FilesHandler : MemoryManager, IFilesReadeble, IStopable
    {
        public FilesHandler() => fileValues = new ConcurrentBag<FileValue>();
        public ConcurrentBag<FileValue> fileValues;
        protected static object locker = new object();
        protected int progress = 0;
        protected bool NeedToStop { get; set; } = false;

        public abstract event FileProgressNotifier ProcessEventNotifier;
        public abstract event FileCompleteNotifier FileCompleteNotifier;

        public virtual Task<ConcurrentBag<FileValue>> CalculateParallel(ConcurrentBag<string> filenames) 
            => Task.Run(() => { return new ConcurrentBag<FileValue>(); });

        protected long CalculateValues(byte[] bytes)
        {
            long summ = 0;
            foreach (var @byte in bytes)
                summ += @byte;
            return summ;
        }

        protected static long GetLength(string fileName)
        {
            using var stream = File.OpenRead(fileName);
            return stream.Length;
        }

        public async virtual  void StopCalculate() => await Task.Run(() => { NeedToStop = true; });
    }
}
