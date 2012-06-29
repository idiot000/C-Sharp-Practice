using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace BackgroundWorker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<int> data=new List<int>();
        List<int> new_data = new List<int>();
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<int> temp = new List<int>();
            int time = (int)e.Argument;
            for (int i = 0; i <= 10; i++)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                backgroundWorker1.ReportProgress(i * 10,i);
                Thread.Sleep(time);
                temp.Add(i);
            }
            e.Result = temp;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show("You have cancelled the backgroundWorker!");
            else
            {
                data.AddRange((List<int>)e.Result);
                MessageBox.Show("Done");
            }
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            backgroundWorker1.RunWorkerAsync(300);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            new_data.Add((int)e.UserState);
        }
    }
}
