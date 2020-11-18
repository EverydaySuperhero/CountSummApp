using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.Interfaces
{
    public interface IFilesReader
    {

        public delegate void EventNotifier(string res, long performed, long maximum);
        public event EventNotifier processEventNotifier;


        Task<List<FileValue>> CalculateParallel(BlockingCollection<string> filenames);

    }
}
