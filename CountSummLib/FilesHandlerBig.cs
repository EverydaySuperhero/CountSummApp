using CountSummLib.Abstract;
using CountSummLib.Exceptions;
using CountSummLib.Interfaces;
using CountSummLib.Models;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib
{
    public class FilesHandlerBig: MemoryManager, IFilesReader
    {
        public delegate void ProcessNotifyer(long performed, long maximum, long res);
        public event ProcessNotifyer processNotifyer;

        public delegate void ProcessNotifier(string res);
        public event ProcessNotifier processEndNotifier;
        public event IFilesReader.EventNotifier processEventNotifier;

        private static long res = 0;
        private static long progress = 0;
        private long fileLength = 0;
        long iterations;
        int partSize;
        BinaryReader reader;

        public async Task<int> CalculateParallel(string filename)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var dt = DateTime.Now;
                    fileLength = GetLength(filename);
                    partSize = GetBlockSize(fileLength); 
                    reader = CreateReader(filename);
                    iterations = (long)Math.Round(fileLength / (decimal)partSize, MidpointRounding.AwayFromZero);

                    Parallel.For(0, iterations, CalculateFilePart);

                    string str = $"File: {Path.GetFileName(filename)}  value: {res} time: {(DateTime.Now - dt).TotalSeconds}";
                    processEndNotifier?.Invoke(str);
                    reader?.Close();
                    reader?.Dispose();
                    reader = null;
                    return (int)res;
                }
                catch (Exception ex)
                {
                    throw new CalculateFileException($"Не удалось высчитать файл: {Path.GetFileName(filename)}", ex);
                }
                finally
                {
                    reader?.Close();
                    reader?.Dispose();
                    reader = null;
                }

            });
        }
        public void CalculateFilePart(long i)
        {
            byte[] bytes = new byte[partSize];
            reader.Read(bytes, 0, partSize);
            var resu = CalculateAmount(bytes);
            res += resu;
            progress += partSize;
            processNotifyer?.Invoke(progress, fileLength, 0);
        }
        private static BinaryReader CreateReader(string filename) => new BinaryReader(new FileStream(filename, FileMode.Open));



        private static long GetLength(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return stream.Length;
            }
        }
        private static int CalculateAmount(byte[] bytes)
        {
            int summ = 0;
            foreach (var @byte in bytes)
            {
                summ += @byte;
            }
            return summ;
        }

        public Task<List<FileValue>> CalculateParallel(BlockingCollection<string> filenames)
        {
            return Task.Run(() => 
            {
                List<FileValue> list = new List<FileValue>();
                foreach (var filename in filenames)
                {
                    try
                    {
                        int res = CalculateParallel(filename).Result;
                        FileValue @struct = new FileValue() { FileName = filename, Summ = res };
                        list.Add(@struct);
                    }
                    catch (Exception e)
                    {
                        FileValue @struct = new FileValue() { FileName = filename, Error=e.Message };
                        list.Add(@struct);
                    }

                }
                return list;
            });

        }
    }
}
