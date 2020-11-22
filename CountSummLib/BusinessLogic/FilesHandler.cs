using CountSummLib.Abstract;
using CountSummLib.Exceptions;
using CountSummLib.Interfaces;
using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace CountSummLib.BusinessLogic
{
    internal class FilesHandlerLittle : FilesHandler
    {
        public override event FileProgressNotifier ProcessEventNotifier;
        public override event FileCompleteNotifier FileCompleteNotifier;
        protected int FilesCount = 0;


        //public event FileProgressNotifier ProcessEventNotifier;
        //public event FileCompleteNotifier FileCompleteNotifier;

        public override async Task<ConcurrentBag<FileValue>> CalculateParallel(ConcurrentBag<string> filenames)
        {
            progress = 0;
            FilesCount = filenames.Count;
            return await Task.Run(() =>
            {

                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                var loop = Parallel.ForEach(filenames, parallelOptions, (filename, stp) => { ReadAndCalculateFile(filename, stp); });
                //foreach(var filename in filenames)
                //{
                //    ReadAndCalculateFile(filename, null);
                //}
                if (NeedToStop)
                    throw new StopException();

                return fileValues;
            });
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

                    var partSize = (blockSize == 0) ? GetBlockSize(fileLength) : blockSize;
                    byte[] bytes;
                    for (int i = 0; i < fileLength; i += partSize)
                    {
                        if (NeedToStop)
                            stp?.Stop();


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
                    ProcessEventNotifier?.Invoke(addInfo, progress, FilesCount);
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
                ProcessEventNotifier?.Invoke(addInfo, progress, FilesCount);
            }
        }
        public override async void StopCalculate() => await Task.Run(() => { NeedToStop = true; });
    }
}
//  private static BinaryReader CreateReader(string filename) => new BinaryReader(new FileStream(filename, FileMode.Open));

//private int GetMaxPartSize()
//{
//    var AvailablePhysicalMemory = (long)(new ComputerInfo().AvailablePhysicalMemory * 0.5f);
//    if (AvailablePhysicalMemory > 2147647)// если выделяемая память меньше макс размера пакета, вернуть макс количество памяти
//    {
//        if (fileLength > AvailablePhysicalMemory)//если размер файла меньше выделяемой памяти, пусть размер пакета = файлу, иначе вернуть макс занчение пакета 
//            return (int)2147647;
//        else
//            return (int)fileLength;
//    }
//    return (int)AvailablePhysicalMemory;
//}