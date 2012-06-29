using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace HostNameInfo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void PrintHostInfo(string host)
        {
            try
            {
                IPHostEntry hostInfo;
                listView1.Items.Clear();
                hostInfo = Dns.GetHostEntry(host);
                ListViewItem item;
                ListViewItem.ListViewSubItem sitem;
                int AList = hostInfo.AddressList.Length;
                int AlList = hostInfo.Aliases.Length;
                listView1.BeginUpdate();
                for(int i = 0;i<(AList>AlList?AList:AlList);i++)
                {
                    item = new ListViewItem();
                    if (i < AList)
                        item.Text = hostInfo.AddressList.ElementAt(i).ToString();
                    if (i < AlList)
                    {
                        sitem = new ListViewItem.ListViewSubItem();
                        sitem.Text = hostInfo.Aliases.ElementAt(i);
                        item.SubItems.Add(sitem);
                    }
                    listView1.Items.Add(item);
                }
                textBox2.Text = "Resolved Successfully!";
                listView1.EndUpdate();
            }
            catch (Exception)
            {
                textBox2.Text = "Unable to Resolve Host!";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "Resolving...";
            textBox2.Update();
            PrintHostInfo(textBox1.Text);
        }
    }
}

