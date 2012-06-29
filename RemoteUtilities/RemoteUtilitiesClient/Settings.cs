using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RemoteUtilitiesClient
{
    public partial class Settings : Form
    {
        public Settings(Form form)
        {
            InitializeComponent();
            parent = form;
            TextWriter tw = new StreamWriter("settings.ini", true);
            tw.Close();
            TextReader tr = new StreamReader("settings.ini");
            String line = null;
            line = tr.ReadLine();
            if (line != null)
            {
                if (!line.StartsWith("current"))
                {
                    textBox1.Text = line;
                    textBox1.Refresh();
                }
                else
                    checkBox1.Checked = true;
            }
            tr.Close();
        }

        Form parent = null;

        private void button2_Click(object sender, EventArgs e)
        {
            TextWriter tw = new StreamWriter("settings.ini", false);
            if (textBox1.Text.Length > 0 && checkBox1.Checked == false)
                tw.Write(textBox1.Text);
            else
                tw.Write("current");
            tw.Close();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                textBox1.Refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked==true)
            {
                textBox1.Enabled = false;
                button1.Enabled = false;
                label1.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
                button1.Enabled = true;
                label1.Enabled = true;
            }
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.Enabled = true;
            this.Dispose();
        }
    }
}
