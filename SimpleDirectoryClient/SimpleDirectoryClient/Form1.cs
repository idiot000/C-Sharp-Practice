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
using System.Net;
using System.Net.Sockets;

namespace SimpleDirectoryClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket sock;
        private void button2_Click(object sender, EventArgs e)
        {
            ListViewItem entry;
            ListViewItem.ListViewSubItem sentry;
            sock.Send(Encoding.ASCII.GetBytes("::dirlist\n\n"), SocketFlags.None);
            string buffer=null;
            string delimiter="\n";
            try
            {
                do
                {
                    buffer = Framer.nextFrame(sock, delimiter);
                    entry = new ListViewItem();
                    entry.Text = buffer;
                    sentry = new ListViewItem.ListViewSubItem();
                    buffer = Framer.nextFrame(sock, delimiter);
                    sentry.Text = buffer;
                    entry.SubItems.Add(sentry);
                    sentry = new ListViewItem.ListViewSubItem();
                    buffer = Framer.nextFrame(sock, delimiter);
                    sentry.Text = buffer;
                    entry.SubItems.Add(sentry);
                    listView1.Items.Add(entry);
                    listView1.Refresh();
                }
                while (buffer.EndsWith("\n")==false);
            }
            catch (SocketException excep)
            {
                MessageBox.Show(excep.Message);
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu listmenu = new ContextMenu();
                MenuItem mopen = new MenuItem();
                MenuItem mdownload = new MenuItem();
                mopen.Text = "Open";
                mdownload.Text = "Download";
                listmenu.MenuItems.Add(mopen);
                listmenu.MenuItems.Add(mdownload);
                Point current = Cursor.Position;
                current = listView1.PointToClient(current);
                listmenu.Show(listView1, current);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0)
            {
                textBox3.Text = "Missing Fields!";
                return;
            }
            try
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(Dns.GetHostAddresses(textBox1.Text), int.Parse(textBox2.Text));
                textBox3.Text = "Connected!";
            }
            catch (SocketException excep)
            {
                MessageBox.Show(excep.Message+"fdgfdg");
            }
        }
    }
}
