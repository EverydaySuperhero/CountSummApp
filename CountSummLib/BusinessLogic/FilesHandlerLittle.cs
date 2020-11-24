using CountSummLib.Abstract;
using CountSummLib.Exceptions;
using CountSummLib.Interfaces;
using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.BusinessLogic
{
    internal class FilesHandlerLittle : FilesHandler
    {
        public override event ProgressNotifier ProcessNotifier;
        public override event FileCompleteNotifier FileCompleteNotifier;
        protected int FilesCount = 0;
        //ConcurrentBag<int> threadIDs = new ConcurrentBag<int>();
        public override async Task<ConcurrentBag<FileValue>> CalculateParallel(ConcurrentBag<string> filenames)
        {
            //try
            //{
            progress = 0;
            FilesCount = filenames.Count;
            return await Task.Run(() =>
            {
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                var loop = Parallel.ForEach(filenames, parallelOptions, (filename, stp) => { ReadAndCalculateFile(filename, stp); });
                if (NeedToStop)
                    throw new StopException("Calculating stopped!");

                return fileValues;
            });
            //}
            //catch (Exception e)
            //{
            //    if (e is StopException)
            //        throw e;
            //    if (e.InnerException is StopException)
            //        throw e.InnerException;
            //    throw e;
            //}
        }

        private void ReadAndCalculateFile(string filename, ParallelLoopState stp, int blockSize = 0)
        {
            try
            {
                long res = 0;
                var dt = DateTime.Now;
                var fileLength = GetLength(filename);
                using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
                {
                    //threadIDs.Add(Thread.CurrentThread.ManagedThreadId);
                    var partSize = (blockSize == 0) ? GetBlockSize(fileLength) : blockSize;
                    byte[] bytes;
                    for (int i = 0; i < fileLength; i += partSize)
                    {
                        if (NeedToStop)
                        {
                            stp?.Stop();
                            GC.Collect();
                            return;
                        }

                        bytes = new byte[partSize];
                        bytes = reader.ReadBytes(bytes.Length);
                        var resu = CalculateValues(bytes);
                        res += resu;
                        GC.Collect();
                    }

                    var addInfo = $" File: {filename}  value: {res}, time: {(DateTime.Now - dt).TotalSeconds}  blockSize:{partSize}";
                    var fv = new FileValue { FilePath = filename, Summ = res, Params = addInfo };

                    lock (locker)
                    {
                        fileValues.Add(fv);
                        progress++;
                    }
                    FileCompleteNotifier?.Invoke(fv, true);
                    ProcessNotifier?.Invoke(addInfo, progress, FilesCount);
                    //threadIDs.(Thread.CurrentThread.ManagedThreadId);
                }

            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException)
                {
                    ReadAndCalculateFile(filename, stp, 1048576);
                    return;
                }

                if (ex is StopException)
                    return;

                FileValue fv;
                var addInfo = $"File: {Path.GetFileName(filename)} - ошибка. {ex.Message}";
                lock (locker)
                {
                    fv = new FileValue() { FilePath = filename, Summ = 0, Error = ex.Message, Params = addInfo };
                    fileValues.Add(fv);
                }
                progress++;
                FileCompleteNotifier?.Invoke(fv, false, addInfo);
                ProcessNotifier?.Invoke(addInfo, progress, FilesCount);
            }
        }
        public override async void StopCalculate() => await Task.Run(() => { NeedToStop = true; });
    }
}
