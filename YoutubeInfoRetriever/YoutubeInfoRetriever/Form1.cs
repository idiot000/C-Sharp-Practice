using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace YoutubeInfoRetriever
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || !(textBox1.Text.ToLowerInvariant().StartsWith("http://www.youtube.com/watch?v=") || textBox1.Text.ToLowerInvariant().StartsWith("www.youtube.com/watch?v=") || textBox1.Text.ToLowerInvariant().StartsWith("youtube.com/watch?v=")))
            {
                MessageBox.Show("Please enter a valid YouTube URL!");
                return;
            }

            richTextBox1.Clear();
            richTextBox1.Text = "Retrieving...";
            richTextBox1.Refresh();
            if (textBox1.Text.ToLowerInvariant().StartsWith("youtube.com/watch?v="))
                textBox1.Text = "http://www." + textBox1.Text;
            if (textBox1.Text.ToLowerInvariant().StartsWith("www.youtube.com/watch?v="))
                textBox1.Text = "http://" + textBox1.Text;
            try
            {
                WebClient wc = new WebClient();
                byte[] response = wc.DownloadData(textBox1.Text);
                string content = Encoding.ASCII.GetString(response);
                content = content.Substring(content.IndexOf("<p id=\"watch-uploader-info\">") + 28);
                int start = content.IndexOf("<") + 9;
                int stop;
                content = content.Substring(start);
                if (!content.StartsWith(@"/user/"))
                {
                    content = content.Substring(content.IndexOf("<") + 1);
                    content = content.Substring(content.IndexOf("<"));
                    start = content.IndexOf("<") + 9;
                    stop = content.IndexOf("class", start) - 2;
                    content = content.Substring(start, stop - start);
                }
                else
                {
                    stop = content.IndexOf("class") - 2;
                    content = content.Substring(0, stop);
                }
                string channel = "http://youtube.com" + content;
                string line1 = "Uploader Channel: " + channel;
                response = wc.DownloadData(channel);
                content = Encoding.ASCII.GetString(response);
                content = content.Substring(content.IndexOf("<span class=\"stat-value\">") + 25);
                string line2 = "\r\nSubscribers: " + content.Substring(0, content.IndexOf("<"));
                content = content.Substring(content.IndexOf("<span class=\"stat-value\">") + 25);
                string line3 = "\r\nVideo views: " + content.Substring(0, content.IndexOf("<"));
                richTextBox1.Text = line1;
                richTextBox1.Text += line2;
                richTextBox1.Text += line3;
                richTextBox1.Refresh();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
            }
        }
    }
}