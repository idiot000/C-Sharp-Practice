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
    public partial class About : Form
    {
        public About(Form form)
        {
            InitializeComponent();
            parent = form;
        }

        Form parent = null;

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void About_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.Enabled = true;
            this.Dispose();
        }
    }
}
