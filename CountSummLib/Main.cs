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
        Report Report;

        public event ProgressNotifier FileProgressNotifier;
        public event FileCompleteNotifier FileCompleteNotifier;
        public string DataFileName;
        public bool isWork = false;

        public  Task CalculateFiles(string filePath,string CalculationStopped=null)
        {
            return  Task.Run(() =>
            {
                try
                {
                    if (isWork) return;
                    isWork = true;
                    ConcurrentBag<string> DirPaths = FolderManager.GetAllFilesFromSubfolder(filePath);
                    var res = Calculate(DirPaths).Result;
                    Report = new XmlDataWriter();
                    Report.ProcessNotifier += Main_ProcessEventNotifier;
                    Report.GroupAndWriteFiles(res,DataFileName??"").GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    if (e.InnerException is StopException)
                        FileProgressNotifier?.Invoke(string.IsNullOrEmpty(CalculationStopped)?"Calculation stopped":CalculationStopped, 0, 100);
                    if (e.InnerException is ReportException)
                        throw e;
                    throw e;

                }
                finally
                {

                    isWork = false;

                    GC.Collect();

                }

            });

        }


        private async Task<ConcurrentBag<FileValue>> Calculate(ConcurrentBag<string> dirPaths)
        {
            return await Task.Run(() =>
            {
                try
                {
                    fileReader = new FilesHandlerLittle();
                    fileReader.ProcessNotifier += Main_ProcessEventNotifier;
                    fileReader.FileCompleteNotifier += Main_FileCompleteNotifier;
                    var a = fileReader.CalculateParallel(dirPaths);
                    return a.Result;
                }
                catch(Exception e)
                {
                    if (e.InnerException is StopException)
                        throw e.InnerException;
                    throw e;
                }
                finally
                {
                    fileReader.ProcessNotifier -= Main_ProcessEventNotifier;
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
