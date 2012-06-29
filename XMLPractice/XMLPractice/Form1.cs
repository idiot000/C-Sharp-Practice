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

namespace XMLPractice
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("miner2.xml");
            XmlNode node = xml.DocumentElement;
            func(node.FirstChild);
        }

        void func(XmlNode node)
        {
            /*if (node is XmlText)
            {
                node = node.ParentNode;
                richTextBox1.Text += node.Name + ": ";
                richTextBox1.Refresh();
                node = node.FirstChild;
                richTextBox1.Text += node.Value + "\r\n";
                return;
            }
            if (node.NextSibling != null && node.NextSibling.Name != node.Name)
                func(node.NextSibling);
            if (node.HasChildNodes)
                    func(node.FirstChild);
            else
            {
                richTextBox1.Text += node.Name + "\r\n";
                richTextBox1.Refresh();
                return;
            }*/
            foreach (XmlNode x in node.SelectNodes("child::*/text()"))
            {
                richTextBox1.Text += x.Value + "\r\n";
                richTextBox1.Refresh();
            }
        }
    }
}
