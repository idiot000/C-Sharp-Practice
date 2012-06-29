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
using System.Net.Sockets;
using System.IO;

namespace SimpleDirectoryServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket client=null;
        string buffer = null;
        string delimiter = "\n";
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, 12345));
                server.Listen(1);
                client = server.Accept();
                while (true)
                {
                    if (client.Available > 0)
                        break;
                }
                buffer = Framer.nextFrame(client, delimiter);
                if (buffer.Equals("::dirlist\n"))
                {
                    DirectoryInfo dir = new DirectoryInfo(@"D:\");
                    DirectoryInfo[] dirInfo = dir.GetDirectories();
                    foreach(DirectoryInfo d in dirInfo)
                    {
                        client.Send(Encoding.ASCII.GetBytes(d.ToString() + "\n"));
                        client.Send(Encoding.ASCII.GetBytes("File Folder\n"));
                        client.Send(Encoding.ASCII.GetBytes("Not Applicable\n"));
                    }
                    client.Send(Encoding.ASCII.GetBytes("\n"));
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message+" Server");
            }
        }
    }
}
