using CountSummLib.Interfaces;
using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.Abstract
{
    public abstract class Report : IStopable, IProgressNotifable
    {
        public abstract event ProgressNotifier ProcessNotifier;

        public async void  StopCalculate() => await Task.Run(() => { NeedToStop = true; });
        protected bool NeedToStop = false;
        protected int progress = 0;
        protected static object locker = new object();
        internal virtual Task GroupAndWriteFiles(ConcurrentBag<FileValue> res, string dataFileName=null)
        {
            throw new NotImplementedException();
        }
    }
}
