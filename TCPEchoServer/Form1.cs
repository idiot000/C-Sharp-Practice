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

namespace TCPEchoServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox3.Text="Idle!";
            textBox1.Text="Server Log";
        }
        private const int BUFSIZE = 32;
        private delegate void text(string txt);
        private void stext0(string txt)
        {
            if (this.textBox2.InvokeRequired)
            {
                text t = new text(stext0);
                this.Invoke(t, new object[] { txt });
            }
            else
            {
                textBox2.Text += txt;
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.ScrollToCaret();
                textBox2.Refresh();
            }
        }
        private void stext1(string txt)
        {
            if (this.textBox3.InvokeRequired)
            {
                text t = new text(stext1);
                this.Invoke(t, new object[] { txt });
            }
            else
                textBox3.Text = txt;
        }
        private void sbutton0(string txt)
        {
            if (button1.InvokeRequired)
            {
                text t = new text(sbutton0);
                this.Invoke(t, new object[] { txt });
            }
            else
                button1.Text = txt;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                button1.Text = "Listen";
                backgroundWorker1.CancelAsync();
            }
            else
            {
                backgroundWorker1.RunWorkerAsync();
                button1.Text = "Stop";
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (textBox4.Text.Length == 0)
            {
                sbutton0("Listen");
                return;
            }
            int servport = Int32.Parse(textBox4.Text);
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Any, servport);
                listener.Start();
                stext0("Server Running!\r\n\r\n");
                stext1("Running!");
            }
            catch (SocketException se)
            {
                stext0(se.Message + "\r\n\r\n");
                stext1("Idle!");
                sbutton0("Listen");
                return;
            }
            byte[] recvbuffer = new byte[BUFSIZE];
            int bytesRcvd;
            while(true)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    listener.Stop();
                    stext1("Idle!");
                    break;
                }
                TcpClient client = null;
                NetworkStream netStream = null;
                try
                {
                    if (listener.Pending())
                    {
                        client = listener.AcceptTcpClient();
                        netStream = client.GetStream();
                        int totalBytesEchoed = 0;
                        while ((bytesRcvd = netStream.Read(recvbuffer, 0, recvbuffer.Length)) > 0)
                        {
                            netStream.Write(recvbuffer, 0, bytesRcvd);
                            stext0("Recieved: " + Encoding.ASCII.GetString(recvbuffer));
                            totalBytesEchoed += bytesRcvd;
                        }
                        stext0("\r\n\r\n");
                        stext0("Total bytes echoed: " + totalBytesEchoed + "\r\n\r\n");
                        netStream.Close();
                        client.Close();
                    }
                }
                catch (Exception excep)
                {
                    stext0(excep.Message + "\r\n\r\n");
                    stext1("Idle!");
                    sbutton0("Listen");
                    netStream.Close();
                }
            }
        }
    }
}
