using CountSummLib.Abstract;
using CountSummLib.Interfaces;
using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CountSummLib.BusinessLogic
{
    internal class FilesHandlerBig : FilesHandler
    {


        private long res = 0;
        private long fileLength = 0;
        long iterations;
        int partSize;
        static FileStream reader;
        string filename;

        public override event FileProgressNotifier ProcessEventNotifier;
        public override event FileCompleteNotifier FileCompleteNotifier;

        public async override Task<ConcurrentBag<FileValue>> CalculateParallel(ConcurrentBag<string> filenames)
        {
            return await Task.Run(() =>
            {
                ConcurrentBag<FileValue> list = new ConcurrentBag<FileValue>();
                foreach (var filename in filenames)
                {
                    try
                    {
                        if (filename == null)
                        {

                        }

                        long res = CalculateParallelPerByte(filename).Result;
                        FileValue @struct = new FileValue() { FilePath = filename, Summ = res };
                        list.Add(@struct);
                    }
                    catch (Exception e)
                    {
                        FileValue @struct = new FileValue() { FilePath = filename, Error = e.Message };
                        list.Add(@struct);
                    }

                }
                return list;
            });

        }

        private void ClearValues()
        {
            res = 0;
            progress = 0;
            fileLength = 0;
            iterations = 0;
            partSize = 0;
        }

        public Task<long> CalculateParallelPerByte(string filename)
        {
            return Task.Run(() =>
           {
               try
               {
                   this.filename = filename;
                   var dt = DateTime.Now;
                   fileLength = GetLength(filename);
                   partSize = GetBlockSize(fileLength);//1048576;//
                                                       //reader = new FileStream(filename, FileMode.Open);
                   iterations = (long)Math.Round(fileLength / (decimal)partSize, MidpointRounding.AwayFromZero);
                   //Parallel.For(0, iterations, CalculateFilePart);
                   reader = new FileStream(filename, FileMode.Open);
                   Parallel.For(0, iterations, i=> { Calculate(i); });
                   string str = $"File: {Path.GetFileName(filename)}  value: {res} time: {(DateTime.Now - dt).TotalSeconds}, length: {fileLength}";
                   var fv = new FileValue() { Params = str, FilePath = filename, Summ = res };
                   ProcessEventNotifier?.Invoke(str, 100, 100);
                   reader?.Close();
                   reader?.Dispose();
                   reader = null;
                   return res;

               }
               catch (Exception ex)
               {
                   //throw new CalculateFileException($"Не удалось высчитать файл: {Path.GetFileName(filename)}", ex);
                   string str = $"Не удалось высчитать файл: " + Path.GetFileName(filename);
                   //ProcessEventNotifier?.Invoke(new FileValue() { Params = str, FileName = filename }, 100, 100);
                   return 0;
               }
               finally
               {
                   reader?.Close();
                   reader?.Dispose();
                   reader = null;
                   ClearValues();

               }

           });
        }


        private void CalculateFilePerByte(long i)
        {
            Calculate(i);

        }
        private void Calculate(long i, int baseBlockSize = 0)
        {
            try
            {
                if (baseBlockSize == 0)
                {
                    byte[] bytes;
                    long resu;
                    bytes = new byte[partSize];
                    lock (locker)
                    {
                        reader.Position = i * partSize;
                        reader.ReadAsync(bytes, 0, baseBlockSize).GetAwaiter();
                    }
                    resu = CalculateValues(bytes);
                    bytes = null;
                    GC.Collect();
                    lock (locker)
                    {
                        res += resu;
                        progress += partSize;
                    }
                    // ProcessEventNotifier?.Invoke(new FileValue(), progress, fileLength);
                }
                else
                {
                    var iterations = (long)Math.Round(partSize / (decimal)baseBlockSize, MidpointRounding.AwayFromZero);
                    for (int j = 0; j < iterations; j++)
                    {
                        byte[] bytes;
                        long resu;
                        bytes = new byte[baseBlockSize];
                        lock (locker)
                        {
                            reader.Position = i * partSize;
                            reader.ReadAsync(bytes, 0, baseBlockSize).GetAwaiter();
                        }
                        resu = CalculateValues(bytes);
                        bytes = null;
                        GC.Collect();
                        lock (locker)
                        {
                            res += resu;
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException)
                {
                    //Calculate(i, 1048576);
                    if (baseBlockSize == 0)
                        Calculate(i, (int)(GetBlockSize(fileLength) * 0.7));
                    else
                        Calculate(i, (int)(baseBlockSize * 0.7));

                }
            }

        }



        public override async void StopCalculate() => await Task.Run(() => { NeedToStop = true; });

    }
}
