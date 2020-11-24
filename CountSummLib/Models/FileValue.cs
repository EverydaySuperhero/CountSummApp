using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CountSummLib.Models
{
    public struct FileValue : INotifyPropertyChanged
    {
        private string filename;
        private long summ;
        private string @params;
        private string error;
        private string filepath;

        [DisplayName("Name")]
        [XmlElement("file")]
        public string FileName
        {
            get { return Path.GetFileName(filepath); }
            set
            {
                filename = value;
                OnPropertyChanged("FileName");
            }
        }

        [DisplayName("Summ of byte values")]
        [XmlElement("value")]
        public long Summ
        {
            get { return summ; }
            set
            {
                summ = value;
                OnPropertyChanged("Summ");
            }
        }
        [XmlIgnore]
        [DisplayName("Info")]
        public string Params
        {
            get { return @params; }
            set
            {
                @params = value;
                OnPropertyChanged("Params");
            }
        }
        [XmlIgnore]
        [DisplayName("Error")]
        public string Error
        {
            get { return error; }
            set
            {
                error = value;
                OnPropertyChanged("Error");
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string FilePath
        {
            get { return filepath; }
            set
            {
                filepath = value;
                OnPropertyChanged("FilePath");
            }
        }
        [XmlIgnore]
        [DisplayName("Folder")]
        public string FolderPath => Path.GetDirectoryName(FilePath);


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    }
}
