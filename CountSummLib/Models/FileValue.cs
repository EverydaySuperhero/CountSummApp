using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CountSummLib.Models
{
    public struct FileValue
    {
        [DisplayName("Name")]
        [XmlElement("file")]
        public string FileName { get; set; }

        [DisplayName("Summ of byte values")]
        [XmlElement("value")]
        public long Summ { get; set; }
        [XmlIgnore]
        [DisplayName("Info")]
        public string Params { get; set; }
        [XmlIgnore]
        [DisplayName("Error")]
        public string Error { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string FilePath { get; set; }
        [XmlIgnore]
        [DisplayName("Folder")]
        public string FolderPath => Path.GetDirectoryName(FilePath);
    }
}
