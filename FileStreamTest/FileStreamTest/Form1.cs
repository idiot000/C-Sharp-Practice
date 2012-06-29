using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileStreamTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string filePath = "";
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                textBox1.Text = filePath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileStream fstream1 = new FileStream(filePath,FileMode.Open,FileAccess.Read);
            int totalBytesRead=0;
            int bytesRead=0;
            try
            {
                byte[] buffer = new byte[100];
                byte[] file = new byte[fstream1.Length];
                progressBar1.Maximum = (int)fstream1.Length;
                while ((bytesRead = fstream1.Read(buffer, 0, 100)) > 0)
                {
                    totalBytesRead += bytesRead;
                    textBox2.Text = totalBytesRead + " bytes read!";
                    textBox2.Refresh();
                    progressBar1.Value = totalBytesRead;
                    progressBar1.Refresh();
                }
                textBox2.Text = totalBytesRead + "bytes read successfully!";
            }
            catch (Exception excep)
            {
                textBox2.Text = "Error!";
                MessageBox.Show(excep.Message);
            }
        }
    }
}
