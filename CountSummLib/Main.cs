using CountSummLib.Abstract;
using CountSummLib.BusinessLogic;
using CountSummLib.Exceptions;
using CountSummLib.Interfaces;
using CountSummLib.Models;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CountSummLib
{

    public class Main
    {
        FilesHandler fileReader;

        public event FileProgressNotifier FileProgressNotifier;
        public event FileCompleteNotifier FileCompleteNotifier;




        public Task CalculateFiles(string filePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    //string[] files = Directory.GetFiles(filePath);//macOS Catalina/
                    ConcurrentBag<string> DirPaths = FolderManager.GetAllFilesFromSubfolder(filePath);
                    var res = Calculate(DirPaths).Result;
                    Report report = new Report();
                    report.GroupAndWriteFiles(res);
                }
                catch (Exception e)
                {
                    if (e.InnerException is StopException)
                        FileProgressNotifier?.Invoke("Calculation stopped", 0, 100);

                }
                GC.Collect();

            });

        }


        private async Task<ConcurrentBag<FileValue>> Calculate(ConcurrentBag<string> dirPaths)
        {
            return await Task.Run(() =>
            {
                try
                {
                    fileReader = new FilesHandlerLittle();
                    fileReader.ProcessEventNotifier += Main_ProcessEventNotifier;
                    fileReader.FileCompleteNotifier += Main_FileCompleteNotifier;
                    return fileReader.CalculateParallel(dirPaths).Result;
                }
                catch(Exception e)
                {
                    if (e is StopException)
                        throw e;
                    if (e.InnerException is StopException)
                        throw e.InnerException;
                    throw e;
                }
                finally
                {
                    fileReader.ProcessEventNotifier -= Main_ProcessEventNotifier;
                    fileReader.FileCompleteNotifier -= Main_FileCompleteNotifier;
                    fileReader = null;
                    dirPaths = new ConcurrentBag<string>();
                    GC.Collect();
                }
            });
        }
        public void StopCalculating() => fileReader?.StopCalculate();


        private void Main_FileCompleteNotifier(FileValue fileValue, bool successful, string err = null)
            => FileCompleteNotifier?.Invoke(fileValue, successful, err);

        private void Main_ProcessEventNotifier(string str, long performed, long maximum)
            => FileProgressNotifier?.Invoke(str, performed, maximum);

    }
}
