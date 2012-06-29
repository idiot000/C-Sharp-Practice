using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace DownloadDataTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpWebRequest mh = (HttpWebRequest)WebRequest.Create(@"http://www.linkedin.com/company/1009");
            mh.AddRange(1000, 2000);
            HttpWebResponse rs = (HttpWebResponse)mh.GetResponse();
            Encoding enc = Encoding.GetEncoding(1252);
            //StreamReader st = new StreamReader(rs.GetResponseStream(), enc);
            //string content = st.ReadToEnd();
            richTextBox1.Text = rs.StatusCode.ToString();
            richTextBox1.Refresh();
            /*WebClient wc = new WebClient();
            byte[] response = wc.DownloadData(@"http://www.linkedin.com/company/1009");
            string content = Encoding.ASCII.GetString(response);
            content = content.Substring(content.IndexOf("<span class=\"country-name\">") + 27);
            content = content.Substring(0, content.IndexOf("<"));
            richTextBox1.Text = content;
            richTextBox1.Refresh();*/
            
        }
    }
}
