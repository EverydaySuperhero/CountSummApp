using CountSummLib.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountSummLib.BusinessLogic
{
    public static class FolderManager
    {
        public static ConcurrentBag<string> GetAllFilesFromSubfolder(string inputDir)
        {
            var files = GetAllFilesFromSubfolders(inputDir);
            return new ConcurrentBag<string>(new ConcurrentQueue<string>(files));
        }

        private static List<string> GetAllFilesFromSubfolders(string inputDir)
        {
            List<string> list = new List<string>();
            try
            {
                var directories = Directory.GetDirectories(inputDir);
                foreach (var directory in Directory.GetDirectories(inputDir))
                {
                    list.AddRange(GetAllFilesFromSubfolder(directory));
                }

            }
            catch (UnauthorizedAccessException e)
            {

            }

            list.AddRange(Directory.GetFiles(inputDir).ToList());

            return list;//macOS Catalina/

        }
    }
}
//UnauthorizedAccessException