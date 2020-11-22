using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CountSummLib.Models
{
    [XmlRoot("item")]
    public struct FileValue
    {
        [XmlElement("file")]
        public string FileName;
        [XmlElement("value")]
        public long Summ;
        [XmlIgnore]
        public string Params;
        [XmlIgnore]
        public string Error;

        [XmlIgnore]
        public string FilePath;
        [XmlIgnore]
        public string FolderPath => Path.GetDirectoryName(FilePath);
    }
}
