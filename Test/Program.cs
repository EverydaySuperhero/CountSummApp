using CountSummLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\\test.txt";
            FileCreator fileCreator = new FileCreator();
            fileCreator.CreateFile(path,10023);

            Console.WriteLine("----");
            FileReader fileReader = new FileReader();
            fileReader.processNotifyer += FileReader_processNotifyer;
            var res = fileReader.CalculateParallel(path).Result;

            Console.WriteLine("----");
            FileReader fileReader1 = new FileReader();
            fileReader1.processNotifyer += FileReader_processNotifyer;
            var res1 = fileReader1.CalculateFor(path).Result;

        }

        private static void FileReader_processNotifyer(long performed, long maximum, long res)
        {
            Console.WriteLine(res);
        }
    }
}
