using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.Interfaces
{
    public delegate void FileProgressNotifier(string str, long performed, long maximum);
    public delegate void FileCompleteNotifier(FileValue fileValue, bool successful,string err=null);

    public interface IFilesReadeble
    {
        public event FileProgressNotifier ProcessEventNotifier;
        public event FileCompleteNotifier FileCompleteNotifier;
        public Task<ConcurrentBag<FileValue>> CalculateParallel(ConcurrentBag<string> filenames);

    }
}
