using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace XML_to_CSV_Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TextWriter tw;
        private delegate void text(string txt);
        string file = null;
        int count = 1;

        private void stext0(string txt)
        {
            if (this.textBox3.InvokeRequired)
            {
                text t = new text(stext0);
                this.Invoke(t, new object[] { txt });
            }
            else
            {
                textBox3.Text = txt;
                textBox3.Refresh();
            }
        }

        List<string> rlist = new List<string>();

        void start()
        {
            tw = new StreamWriter(textBox2.Text, true);
            backgroundWorker1.RunWorkerAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0)
            {
                MessageBox.Show("Select both source and destination file path!");
                return;
            }
            file = textBox1.Text.Substring(textBox1.Text.LastIndexOf(@"\") + 1);
            start();
        }

        void init(XmlNode node, bool flag = false)
        {
            if (node is XmlText)
            {
                node = node.ParentNode;
                if ((node.Name == "code" && node.ParentNode.Name == "employee-count-range")|| node.Name=="phone1")
                    return;
                if (flag == false)
                    tw.Write(",");
                else
                    flag = false;
                if (node.Name == "street1")
                {
                    tw.Write("Address");
                    return;
                }
                if (node.Name == "code" && node.ParentNode.Name == "industry")
                {
                    tw.Write("Industry");
                    return;
                }
                if (node.Name == "name" && node.ParentNode.Name == "company")
                {
                    tw.Write("Company Name");
                    return;
                }
                if (node.Name == "website-url")
                {
                    tw.Write("Website");
                    return;
                }
                if (node.Name == "description")
                {
                    tw.Write("Description");
                    return;
                }
                if (node.Name == "founded-year")
                {
                    tw.Write("Year Founded");
                    return;
                }
                if (node.Name == "name" && node.ParentNode.Name == "employee-count-range")
                {
                    tw.Write("Employee Count");
                    return;
                }
            }
            if (node.HasChildNodes)
                init(node.FirstChild, flag);
            else
            {
                if (flag == false && node.Name == "street1" || (node.Name == "code" && node.ParentNode.Name == "industry") || node.Name == "name" || node.Name == "website-url" || node.Name == "description" || node.Name == "founded-year")
                    tw.Write(",");
                else
                    flag = false;
                if (node.Name == "street1")
                    tw.Write("Address");
                if (node.Name == "code" && node.ParentNode.Name == "industry")
                    tw.Write("Industry");
                if (node.Name == "name" && node.ParentNode.Name == "company")
                    tw.Write("Company Name");
                if (node.Name == "website-url")
                    tw.Write("Website");
                if (node.Name == "description")
                    tw.Write("Description");
                if (node.Name == "founded-year")
                    tw.Write("Year Founded");
                if (node.Name == "name" && node.ParentNode.Name == "employee-count-range")
                    tw.Write("Employee Count");
            }
            if (node.Name != "street1" && !(node.Name == "code" && node.ParentNode.Name == "industry") && node.Name != "phone1")
                if (node.NextSibling != null && node.NextSibling.Name != node.Name)
                    init(node.NextSibling);
        }

        void xmlParse(XmlNode node, bool flag = false)
        {
            if (node.Name == "industries")
            {
                tw.Write(",\"");
                foreach (XmlNode parent in node.SelectNodes("*"))
                {
                    XmlNodeList x = parent.SelectNodes("child::*/text()");
                    tw.Write(x[1].Value + "\r\n");
                }
                tw.Write("\"");
                if (node.NextSibling != null && node.NextSibling.Name != node.Name)
                    xmlParse(node.NextSibling);
                return;
            }
            if (node.Name == "employee-count-range")
            {
                XmlNodeList x = node.SelectNodes("child::*/text()");
                string temp = null;
                if (x[1].Value.Contains("-"))
                {
                    temp = x[1].Value.Insert(x[1].Value.IndexOf("-"), "<");
                    temp = temp.Insert(temp.IndexOf("-") + 1, "->");
                }
                else
                    temp = x[1].Value;
                tw.Write("," + "\"" + temp + "\"");
                if (node.NextSibling != null && node.NextSibling.Name != node.Name)
                    xmlParse(node.NextSibling);
                return;
            }
            if (node.Name == "locations")
            {
                tw.Write(",");
                foreach (XmlNode parent in node.SelectNodes("*"))
                {
                    tw.Write("\"");
                    foreach (XmlNode x in parent.SelectNodes("*"))
                    {
                        foreach (XmlNode y in x.SelectNodes("child::*/text()"))
                        {
                            string temp = y.Value.Replace("\"", " ");
                            if (y.ParentNode.Name == "street1")
                                tw.Write("street" + ": " + temp + "\r\n");
                            else
                            {
                                if (y.ParentNode.Name == "phone1")
                                    tw.Write("phone" + ": " + temp + "\r\n");
                                else
                                    tw.Write(y.ParentNode.Name + ": " + temp + "\r\n");
                            }
                            
                        }
                    }
                    tw.Write("\r\n\"");
                }
                if (node.NextSibling != null && node.NextSibling.Name != node.Name)
                    xmlParse(node.NextSibling);
                return;
            }
            if (node is XmlText)
            {
                if (flag == true)
                {
                    tw.Write("\r\n");
                    flag = false;
                }
                else
                    tw.Write(",");
                string temp = node.Value.Replace("\"", " ");
                tw.Write("\"" + temp + "\"");
                return;
            }
            if (node.HasChildNodes)
                xmlParse(node.FirstChild, flag);
            else
            {
                if (flag == true)
                {
                    tw.Write("\r\n");
                    flag = false;
                }
                else
                    tw.Write(",");
            }
            if (node.NextSibling != null && node.NextSibling.Name != node.Name)
                xmlParse(node.NextSibling);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                textBox1.Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = saveFileDialog1.FileName;
                textBox2.Refresh();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            XmlDocument srcXml = new XmlDocument();
            srcXml.Load(textBox1.Text);
            long rows = 0;
            long curr = 0;
            foreach (XmlNode x in srcXml.DocumentElement.ChildNodes)
            {
                rows++;
                stext0(rows.ToString() + " rows!");
            }
            if (count == 1)
                init(srcXml.DocumentElement.FirstChild, true);
            foreach (XmlNode x in srcXml.DocumentElement.ChildNodes)
            {
                xmlParse(x, true);
                curr++;
                backgroundWorker1.ReportProgress((int)((curr / rows) * 100));
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Refresh();
            if (e.ProgressPercentage == 100)
            {
                //MessageBox.Show("Conversion completed successfully!");
                tw.Close();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (count > 67)
                return;
            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.LastIndexOf(@"\") + 1);
            textBox1.Text += file.Substring(0, file.LastIndexOf("r") + 1) + (++count).ToString() + ".txt";
            textBox2.Text = textBox2.Text.Substring(0, textBox2.Text.LastIndexOf(@"\") + 1);
            textBox2.Text += file.Substring(0, file.LastIndexOf("r") + 1) + (count).ToString() + ".csv";
            start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            file = textBox1.Text.Substring(textBox1.Text.LastIndexOf(@"\") + 1);
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            XmlDocument srcXml = new XmlDocument();
            srcXml.Load(textBox1.Text);
            long rows = 0;
            if (count > 1)
            {
                foreach (XmlNode x in srcXml.DocumentElement.ChildNodes)
                {
                    if (rlist.Contains(x.FirstChild.FirstChild.Value))
                    {
                        stext0("Comparing and deleting " + rows.ToString() + " rows!");
                        MessageBox.Show(x.FirstChild.FirstChild.Value + " deleted!");
                        srcXml.DocumentElement.RemoveChild(x);
                    }
                }
            }
            rlist.Clear();
            foreach (XmlNode x in srcXml.DocumentElement.ChildNodes)
            {
                rows++;
                stext0("Loading " + rows.ToString() + " rows!");
                rlist.Add(x.FirstChild.FirstChild.Value);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (count > 67)
            {
                count = 1;
                return;
            }
            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.LastIndexOf(@"\") + 1);
            textBox1.Text += file.Substring(0, file.LastIndexOf("r") + 1) + (++count).ToString() + ".txt";
            backgroundWorker2.RunWorkerAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
