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

namespace UDPEchoServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "Server Log";
        }
        private delegate void text(string txt);
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
            UdpClient client = null;
            try
            {
                client = new UdpClient(Int32.Parse(textBox3.Text));
                stext2("Running\r\n");
                stext4("Running!");
            }
            catch (SocketException se)
            {
                stext2(se.ErrorCode + ": " + se.Message + "\r\n\r\n");
                stext4("Idle!");
                sbutton("Listen");
                return;
            }
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    stext2("Stopped\r\n\r\n");
                    stext4("Idle!");
                    client.Close();
                    break;
                }
                try
                {
                    if (client.Available > 0)
                    {
                        byte[] byteBuffer = client.Receive(ref remoteIPEndPoint);
                        stext2("Handling client at: " + remoteIPEndPoint.Address + ":" + remoteIPEndPoint.Port + "\r\n");
                        stext2("Recieved: " + Encoding.ASCII.GetString(byteBuffer) + "\r\n");
                        client.Send(byteBuffer, byteBuffer.Length, remoteIPEndPoint);
                        stext2("Echoed " + byteBuffer.Length + " bytes\r\n\r\n");
                    }
                }
                catch (SocketException se)
                {
                    stext2(se.ErrorCode + ": " + se.Message + "\r\n\r\n");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                if (textBox3.Text.Length == 0)
                {
                    textBox4.Text = "No Port Entered!";
                    return;
                }
                button1.Text = "Stop";
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                button1.Text = "Listen";
                backgroundWorker1.CancelAsync();
            }
        }
    }
}
