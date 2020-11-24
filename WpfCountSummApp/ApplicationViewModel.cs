using CountSummLib;
using CountSummLib.Exceptions;
using CountSummLib.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfCountSummApp
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<FileValue> FileValues { get; set; }
        public ApplicationViewModel()
        {
            Main.FileProgressNotifier += Main_FileProgressNotifier;
            Main.FileCompleteNotifier += Main_FileCompleteNotifier;
            FileValues = new ObservableCollection<FileValue>();
        }

        Main Main = new Main();
        private int currentProgress;
        public int CurrentProgress
        {
            get { return currentProgress; }
            private set
            {
                if (currentProgress != value)
                {
                    currentProgress = value;
                    OnPropertyChanged("CurrentProgress");
                }
            }
        }
        string progressBarStatusText;
        public string ProgressBarStatusText
        {
            get { return progressBarStatusText; }
            set
            {
                progressBarStatusText = value;
                OnPropertyChanged("ProgressBarStatusText");
            }
        }
        string statusText;
        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        private RelayCommand stopBtn;
        public RelayCommand StopBtn
        {
            get
            {
                return stopBtn ??
                  (stopBtn = new RelayCommand(obj =>
                  {
                      Main.StopCalculating();
                  }));
            }
        }
        private RelayCommand selectFolderAndRunBtn;
        public RelayCommand SelectFolderAndRunBtn
        {
            get
            {
                
                return selectFolderAndRunBtn ??
                  (selectFolderAndRunBtn = new RelayCommand(obj =>
                  {
                      if (Main.isWork)
                      {
                          MessageBox.Show(Properties.Resources.IsBusyWorker, Properties.Resources.Error);
                          return;
                      }
                      using (var fbd = new FolderBrowserDialog())
                      {
                          DialogResult result = fbd.ShowDialog();

                          if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                          {
                              try
                              {
                                  Main.CalculateFiles(fbd.SelectedPath, Properties.Resources.CalculationStopped);

                              }
                              catch (Exception e)
                              {

                                  if(e.InnerException is ReportException)
                                      MessageBox.Show(e.InnerException.InnerException.Message, Properties.Resources.Error);
                                  else
                                      MessageBox.Show(e.InnerException.Message, Properties.Resources.Error);


                              }
                          }
                      }
                  }));
            }
        }

        private void Main_FileProgressNotifier(string str, long performed, long maximum)
        {

            ProgressBarStatusText = performed + " of " + maximum;
            StatusText = str;

            var result = (performed * 100 / maximum);
            if (result > 100)
                result = 100;
            CurrentProgress = (int)result;

        }

        private void Main_FileCompleteNotifier(FileValue fileValue, bool successful, string err = null)
        {

            App.Current.Dispatcher.Invoke(delegate // <--- HERE
            {
                FileValues.Add(fileValue);
            });

        }

        public FileValue SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        private FileValue selectedItem;


        public void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
