using CountSummLib.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CountSummLib.Interfaces
{
    public delegate void ProgressNotifier(string str, long performed, long maximum);
    public delegate void FileCompleteNotifier(FileValue fileValue, bool successful,string err=null);

    public interface IFilesReadeble
    {
        public event FileCompleteNotifier FileCompleteNotifier;
        public Task<ConcurrentBag<FileValue>> CalculateParallel(ConcurrentBag<string> filenames);

    }
}
