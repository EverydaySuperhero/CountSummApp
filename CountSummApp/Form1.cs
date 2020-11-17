using CountSummLib;
using Microsoft.VisualBasic.Devices;
using System;
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
        public Form1()
        {
            InitializeComponent();

            var asd = (long)new ComputerInfo().AvailablePhysicalMemory;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            Calculate(openFileDialog1.FileName);

        }


        private void Calculate(string filename)
        {
            FileReader fileReader = new FileReader();
            fileReader.processEndNotifier += FileReader_processNotifyer;
            fileReader.processNotifyer += SetProgressBar;
            //fileReader.CalculateFull(filename);
            fileReader.CalculateParallel(filename);

            //fileReader.CalculateParallel(filename);
        }

        private void FileReader_processNotifyer(string res)
        {

            BeginInvoke((Action)(() =>
            {
                MessageBox.Show("значение: " + res + ".");
                richTextBox1.AppendText("res: "+res+"\n");
            }));
        }
        long Result=0;

        private void SetProgressBar(long performed, long maximum,long byteRes)
        {
            BeginInvoke((Action)(() =>
            {
                var res = (int)(performed * 100 / maximum);
                Result += byteRes;
                label1.Text = Result.ToString();
                if (res > 100)
                    res = 100;
                progressBar1.Value = res;
                
            }));
        }

    }
}

