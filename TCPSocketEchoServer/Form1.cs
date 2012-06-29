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

namespace TCPSocketEchoServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "Server Log";
        }
        private delegate void text(string txt);
        private const int BUFSIZE = 32;
        private const int BACKLOG = 5;
        private void stext2(string txt)
        {
            if (textBox2.InvokeRequired)
            {
                text t = new text(stext2);
                Invoke(t, new object[] { txt });
            }
            else
            {
                textBox2.Text += txt;
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.ScrollToCaret();
                textBox2.Refresh();
            }
        }
        private void stext4(string txt)
        {
            if (textBox4.InvokeRequired)
            {
                text t = new text(stext4);
                Invoke(t, new object[] { txt });
            }
            else
            {
                textBox4.Text = txt;
                textBox4.Refresh();
            }
        }
        private void sbutton(string txt)
        {
            if (button1.InvokeRequired)
            {
                text t = new text(sbutton);
                Invoke(t, new object[] { txt });
            }
            else
                button1.Text = txt;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Socket server = null;
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text)));
                server.Listen(BACKLOG);
                stext2("Running\r\n");
                stext4("Running!");
                sbutton("Stop");
            }
            catch (SocketException se)
            {
                stext2(se.ErrorCode + ": " + se.Message + "\r\n\r\n");
                stext4("Idle!");
                sbutton("Listen");
                return;
            }
            byte[] recvBuffer = new byte[BUFSIZE];
            int bytesRcvd;
            while (true)
            {
                Socket client = null;
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    sbutton("Listen");
                    stext2("Stopped\r\n\r\n");
                    stext4("Idle");
                    server.Close();
                    return;
                }
                try
                {
                    client = server.Accept();
                    stext2("Handling client at: " + client.RemoteEndPoint.ToString() + "\r\n\r\n");
                    int totalBytesEchoed = 0;
                    while ((bytesRcvd = client.Receive(recvBuffer, recvBuffer.Length, SocketFlags.None)) > 0)
                    {
                        client.Send(recvBuffer, 0, recvBuffer.Length, SocketFlags.None);
                        totalBytesEchoed += bytesRcvd;
                        if (client.Available == 0)
                            break;
                    }
                    stext2(totalBytesEchoed + " bytes echoed\r\n\r\n");
                    client.Close();
                }
                catch (Exception excep)
                {
                    stext2(excep.Message + "\r\n\r\n");
                    client.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                if(textBox3.Text.Length==0)
                    stext4("No Port Entered!");
                else
                    backgroundWorker1.RunWorkerAsync();
            else
            {
                backgroundWorker1.CancelAsync();
                button1.Text = "Listen";
            }
        }
    }
}
