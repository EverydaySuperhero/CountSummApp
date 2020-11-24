using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CountSummLib;
using CountSummLib.BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCuontSumm
{
    [TestClass]
    public class UnitTest1
    {
        private const string base_dir = "D:\\CountSummAppTestDir";
        [TestMethod]
        public void TestMethod1()
        {
            //FileCreator fileCreator = new FileCreator();
            //DirectoryInfo CountSummAppTest_dir = Directory.CreateDirectory(base_dir);
            //var TestFileSumm = fileCreator.CreateFile(CountSummAppTest_dir.FullName+"\\TestFile.txt", 2146435070);
            //var TestFileSumm1 = fileCreator.CreateFile(CountSummAppTest_dir.FullName+"\\TestFile1.txt", 42737824);

            //DirectoryInfo CountSummAppTest_subdir_dir = Directory.CreateDirectory("D:\\CountSummAppTestDir\\subdir");
            //var TestFileSumm2 = fileCreator.CreateFile(CountSummAppTest_subdir_dir.FullName + "\\TestFile.txt", 74827494);
            //var TestFileSumm3 = fileCreator.CreateFile(CountSummAppTest_subdir_dir.FullName + "\\TestFile1.txt", 42737824);

            //var list = new List<string>() { CountSummAppTest_dir.FullName + "\\TestFile.txt", CountSummAppTest_dir.FullName + "\\TestFile1.txt", CountSummAppTest_subdir_dir.FullName + "\\TestFile.txt", CountSummAppTest_subdir_dir.FullName + "\\TestFile1.txt" };
            //FilesHandlerLittle filesHandlerLittle = new FilesHandlerLittle();
            //var res = filesHandlerLittle.CalculateParallel(new ConcurrentBag<string>(list)).Result;

            //Assert.AreEqual(TestFileSumm, res.Where(w => w.FilePath == CountSummAppTest_dir.FullName + "\\TestFile.txt").FirstOrDefault().Summ);

            //Assert.AreEqual(TestFileSumm1, res.Where(w => w.FilePath == CountSummAppTest_dir.FullName + "\\TestFile1.txt").FirstOrDefault().Summ);

            //Assert.AreEqual(TestFileSumm2, res.Where(w => w.FilePath == CountSummAppTest_subdir_dir.FullName + "\\TestFile.txt").FirstOrDefault().Summ);

            //Assert.AreEqual(TestFileSumm3, res.Where(w => w.FilePath == CountSummAppTest_subdir_dir.FullName + "\\TestFile1.txt").FirstOrDefault().Summ);

            //CountSummAppTest_dir.Delete(true);
        }
    }
}
