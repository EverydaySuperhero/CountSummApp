using CountSummLib;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CountSummApp
{
    public partial class Form1 : Form
    {
        public Form1() => InitializeComponent();


        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    if (files.Length == 0 && Directory.GetDirectories(fbd.SelectedPath).Length == 0)
                    {
                        MessageBox.Show("Selected folder doesn't contains any files of directories.","Error");
                        return;
                    }
                    BlockingCollection<string> list = new BlockingCollection<string>(new ConcurrentQueue<string>(files.ToList()));
                    Calculate(list);
                }
            }


        }


        private void Calculate(BlockingCollection<string> dirPath)
        {
            FilesHandler fileReader = new FilesHandler();
            fileReader.processEventNotifier += FileReader_processNotifyer;
            var res = fileReader.CalculateParallel(dirPath).Result;
        }

        private void FileReader_processNotifyer(string res, long performed, long maximum)
        {
            BeginInvoke((Action)(() =>
            {
                //MessageBox.Show("значение: " + res + ".");
                richTextBox1.AppendText(res + "\n");

                var result = (int)(performed * 100 / maximum);
                if (result > 100)
                    result = 100;
                progressBar1.Value = result;
            }));
        }
    }
}

