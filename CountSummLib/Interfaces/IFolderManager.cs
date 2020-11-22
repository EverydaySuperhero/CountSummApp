using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.Interfaces
{
    public interface IFolderManager
    {
        public BlockingCollection<string> GetAllFilesFromSubfolder(string inputDir);
    }
}
