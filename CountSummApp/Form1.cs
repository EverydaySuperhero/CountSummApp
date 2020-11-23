using CountSummLib;
using CountSummLib.BusinessLogic;
using CountSummLib.Interfaces;
using CountSummLib.Models;
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
        public Form1()
        {
            InitializeComponent();
            Main.FileCompleteNotifier += Main_FileCompleteNotifier;
            Main.FileProgressNotifier += Main_FileProgressNotifier;
        }

        BindingList<FileValue> fileValues = new BindingList<FileValue>();



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            dataGridView1.DataSource = new BindingSource(fileValues, null);
        }

        Main Main = new Main();
        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    fileValues.Clear();
                    progressBar1.Value = 0;
                    Main.CalculateFiles(fbd.SelectedPath);
                }
            }
        }

        private void Main_FileProgressNotifier(string str, long performed, long maximum)
        {
            BeginInvoke((Action)(() =>
            {
                label1.Text = performed + " of " + maximum;

                var result = (performed * 100 / maximum);
                if (result > 100)
                    result = 100;
                progressBar1.Value = (int)result;

            }));

            
        }

        private void Main_FileCompleteNotifier(FileValue fileValue, bool successful, string err = null)
        {
            BeginInvoke((Action)(() =>
            {
                fileValue.FileName = Path.GetFileName(fileValue.FilePath);
                fileValues.Add(fileValue);
                //dataGridView1.DataSource = new BindingList<FileValue>(fileValues);

                //MessageBox.Show("значение: " + res + ".");
                if (!string.IsNullOrEmpty(fileValue.Params))
                {
                    //richTextBox1.AppendText(fileValue.Params + "\n");
                    //if(richTextBox1.Lines.Length!=0)
                    //richTextBox1.SelectionStart = richTextBox1.Find(richTextBox1.Lines[richTextBox1.Lines.Length-1]);
                    //richTextBox1.ScrollToCaret();

                }
               
            }));
        }



        private void Stop_btn_Click(object sender, EventArgs e)
        {
            Main.StopCalculating();
        }
    }
}

