using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib
{
    public class FileReader
    {
        public delegate void ProcessNotifyer(long performed, long maximum, long res);
        public event ProcessNotifyer processNotifyer;

        public delegate void ProcessNotifier(string res);
        public event ProcessNotifier processEndNotifier;

        private static long res = 0;
        private static long progress = 0;
        private long fileLength = 0;
        long iterations;
        int partSize;
        BinaryReader reader;






        public async void CalculateParallel(string filename)
        {
            await Task.Run(() =>
            {
                try
                {
                    var dt = DateTime.Now;
                    fileLength = GetLength(filename);
                    partSize = GetMaxPartSize();
                    reader = CreateReader(filename);
                    iterations = (long)Math.Round(fileLength / (decimal)partSize, MidpointRounding.AwayFromZero);

                    Parallel.For(0, iterations, CalculateFilePart);

                    string str = $"File: {Path.GetFileName(filename)}  value: {res} time: {(DateTime.Now - dt).TotalSeconds}";
                    processEndNotifier?.Invoke(str);
                    reader?.Close();
                    reader?.Dispose();
                    reader = null;
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

        private int GetMaxPartSize()
        {
            var AvailablePhysicalMemory = (long)(new ComputerInfo().AvailablePhysicalMemory * 0.5f);
            if (AvailablePhysicalMemory > 2147647)// если выделяемая память меньше макс размера пакета, вернуть макс количество памяти
            {
                if (fileLength > AvailablePhysicalMemory)//если размер файла меньше выделяемой памяти, пусть размер пакета = файлу, иначе вернуть макс занчение пакета 
                    return 2147647;
                else
                    return (int)fileLength;
            }
            else
                return (int)AvailablePhysicalMemory;
        }



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
    }
}

//public async void CalculateFull(string filename)
//{
//    await Task.Run(() =>
//    {
//        var dt = DateTime.Now;
//        var file = new FileInfo(filename);
//        long size = file.Length;

//        var list = new List<string>();
//        length = GetLength(filename);

//        partSize = GetMaxPartSize();
//        var reader = new BinaryReader(new FileStream(filename, FileMode.Open));

//        byte[] bytes = new byte[length];
//        reader.Read(bytes, 0, (int)length);
//        res += CalculateAmount(bytes);

//        var dtEnd = (DateTime.Now - dt).TotalSeconds;

//        processEndNotifier?.Invoke(res.ToString());
//        reader.Close();
//        reader.Dispose();
//        reader = null;
//    });
//}
//public async void CalculateFor(string filename)
//{
//    await Task.Run(() =>
//    {
//        var dt = DateTime.Now;
//        var file = new FileInfo(filename);
//        long size = file.Length;

//        var list = new List<string>();
//        length = GetLength(filename);

//        partSize = 100;
//        var reader = new BinaryReader(new FileStream(filename, FileMode.Open));

//        long lgth = (long)Math.Round(length / (decimal)partSize, MidpointRounding.AwayFromZero);
//        for (int i = 0; i < lgth; i++)
//        {
//            byte[] bytes = new byte[partSize];
//            reader.Read(bytes, 0, partSize);
//            res += CalculateAmount(bytes);
//        }

//        var dtEnd = (DateTime.Now - dt).TotalSeconds;

//        processEndNotifier?.Invoke(res.ToString());
//        reader.Close();
//        reader.Dispose();
//        reader = null;
//    });
//}