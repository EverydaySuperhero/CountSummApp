using CountSummLib.Abstract;
using CountSummLib.Interfaces;
using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static CountSummLib.Interfaces.IFilesReader;

namespace CountSummLib
{
    public class FilesHandler :MemoryManager, IFilesReader
    {
        public event EventNotifier processEventNotifier;

        public List<FileValue> fileValues;
        private static object locker = new object();
        private int progress = 0;
        private int FilesCount = 0;

        public FilesHandler()
        {
            fileValues = new List<FileValue>();
        }



        public async Task<List<FileValue>> CalculateParallel(BlockingCollection<string> filenames)
        {
            return await Task.Run(() => 
            {
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                FilesCount = filenames.Count;
                Parallel.ForEach(filenames.GetConsumingEnumerable(), parallelOptions, filename =>
                {
                    try
                    {
                        int res = 0;
                        var dt = DateTime.Now;
                        var fileLength = GetLength(filename);
                        using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
                        {
                            var partSize = GetBlockSize(fileLength);

                            for (int i = 0; i < fileLength; i += partSize)
                            {
                                byte[] bytes = new byte[partSize];
                                bytes = reader.ReadBytes(bytes.Length);
                                var resu = CalculateAmount(bytes);
                                res += resu;
                            }

                            var fv = new FileValue { FileName = Path.GetFileName(filename), Summ = res };

                            lock (locker)
                            {
                                fileValues.Add(fv);
                                progress++;
                            }
                            processEventNotifier?.Invoke($"File: {Path.GetFileName(filename)}  value: {res},time: {(DateTime.Now - dt).TotalSeconds}  blockSize:{partSize}", progress, FilesCount);
                        } 

                    }
                    catch (Exception ex)
                    {
                        //throw new CalculateFileException($"Не удалось высчитать файл: {Path.GetFileName(filename)}", ex);
                        lock (locker)
                        {
                            var fv = new FileValue() { FileName = Path.GetFileName(filename), Summ = 0,Error = ex.Message };
                            fileValues.Add(fv);
                        }
                        progress++;
                        processEventNotifier?.Invoke($"File: {Path.GetFileName(filename)} - ошибка.", FilesCount, 0);

                    }
                });
                return fileValues;
            });
        


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



        private static long GetLength(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return stream.Length;
            }
        }
        private int CalculateAmount(byte[] bytes)
        {
            int summ = 0;
            foreach (var @byte in bytes)
            {
                summ += @byte;
            }
            return summ;
        }
    }
}
