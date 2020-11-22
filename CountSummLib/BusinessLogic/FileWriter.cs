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
using System.Xml.Serialization;

namespace CountSummLib.BusinessLogic
{
    public class Report : IStopable
    {
        public void StopCalculate()
        {
            NeedToStop = true;
        }
        private bool NeedToStop = false;
        internal Task GroupAndWriteFiles(ConcurrentBag<FileValue> res)
        {
            return Task.Run(() =>
            {
                try
                {
                    var foldersList = res.GroupBy(x => x.FolderPath).Select(s => s.Key).ToList();
                    var dataFile = new ConcurrentDictionary<string, List<FileValue>>();
                    foreach (var folder in foldersList)
                    {
                        dataFile.GetOrAdd(folder, res.Where(w => w.FolderPath == folder && string.IsNullOrEmpty(w.Error)).ToList());
                    }
                    var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                    var loop = Parallel.ForEach(dataFile, parallelOptions, (item,stp) =>
                    {
                        if(NeedToStop) stp.Stop();

                        item.Value.ForEach(itm =>{
                            itm.FileName = Path.GetFileName(itm.FilePath);
                        });

                        XmlSerializer writer = new XmlSerializer(item.Value.GetType());
                        using (var sw = new StreamWriter(item.Key + "//CountSummDataFile.xml"))
                        {
                            writer.Serialize(sw, item.Value);
                        }
                    });
                }
                catch (Exception e)
                {
                    throw new ReportException(e.Message);
                }
            });
        }

    }

}
