using CountSummLib.Abstract;
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
    internal class XmlDataWriter : Report
    {
        public override event ProgressNotifier ProcessNotifier;

        internal override Task GroupAndWriteFiles(ConcurrentBag<FileValue> res, string dataFileName = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    dataFileName = string.IsNullOrEmpty(dataFileName) ? "CountSummDataFile" : dataFileName;
                    progress = 0;

                    var dataFile = new ConcurrentDictionary<string, List<FileValue>>();

                    ConcurrentBag<string> foldersList = new ConcurrentBag<string>(res.GroupBy(x => x.FolderPath).Select(s => s.Key).ToList());

                    foreach (var folder in foldersList)
                    {
                        dataFile.GetOrAdd(folder, res.Where(w => w.FolderPath == folder && string.IsNullOrEmpty(w.Error)).ToList());
                    }

                    var exceptions = new ConcurrentQueue<Exception>();

                    var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                    var loop = Parallel.ForEach(dataFile, parallelOptions, (item, stp) =>
                    {
                        try
                        {
                            
                            if (NeedToStop) stp.Stop();

                            item.Value.ForEach(itm =>
                            {
                                itm.FileName = Path.GetFileName(itm.FilePath);
                            });

                            XmlSerializer writer = new XmlSerializer(item.Value.GetType());
                            using (var sw = new StreamWriter(item.Key + "//" + dataFileName + ".xml"))
                            {
                                writer.Serialize(sw, item.Value);
                            }
                            lock (locker)
                            {
                                progress++;
                            }
                            ProcessNotifier?.Invoke(null, progress, dataFile.Count);

                        }
                        catch (Exception e)
                        {

                            exceptions.Enqueue(e);
                        }
                    });
                    if (exceptions.Count > 0) throw new AggregateException(exceptions);
                }
                catch (Exception e)
                {
                    throw new ReportException(e);
                }
            });
        }

    }

}
