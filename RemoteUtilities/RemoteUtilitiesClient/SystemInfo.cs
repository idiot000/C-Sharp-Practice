using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RemoteUtilitiesClient
{
    public partial class SystemInfo : Form
    {
        public SystemInfo(Form form, List<string> sysinfo)
        {
            InitializeComponent();
            List<string> info = sysinfo;
            label2.Text = info.ElementAt(0);
            label3.Text = info.ElementAt(1);
            label4.Text = info.ElementAt(2);
            label5.Text = info.ElementAt(3);
            label6.Text = info.ElementAt(4);
            parentForm = form;
        }

        Form parentForm;

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SystemInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Enabled = true;
            this.Dispose();
        }
    }
}
