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
using System.Threading;

namespace TCPSocketEchoClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "Client Log";
        }
        private void stext2(string txt)
        {
            textBox2.Text += txt;
            textBox2.SelectionStart = textBox2.Text.Length;
            textBox2.ScrollToCaret();
            textBox2.Refresh();
        }
        private void stext6(string txt)
        {
            textBox6.Text=txt;
            textBox6.Refresh();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 0 || textBox4.Text.Length == 0 || textBox5.Text.Length == 0)
            {
                stext6("Missing Fields!");
                return;
            }
            byte[] byteBuffer = Encoding.ASCII.GetBytes(textBox5.Text);
            Socket sock = null;
            try
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint serverEndPoint = new IPEndPoint(Dns.GetHostAddresses(textBox3.Text).ElementAt(0), Int32.Parse(textBox4.Text));
                stext2("Connecting...\r\n");
                stext6("Connecting...");
                sock.Connect(serverEndPoint);
                stext2("Connected to server! - Sending echo string...\r\n");
                stext6("Sending...");
                sock.Send(byteBuffer, 0, byteBuffer.Length, SocketFlags.None);
                stext2(byteBuffer.Length + " bytes sent to server\r\n");
                int totalBytesRcvd = 0;
                int bytesRcvd = 0;
                stext6("Recieving...");
                while (totalBytesRcvd < byteBuffer.Length)
                {
                    if ((bytesRcvd = sock.Receive(byteBuffer, totalBytesRcvd, byteBuffer.Length - totalBytesRcvd, SocketFlags.None)) == 0)
                    {
                        stext2("Connection terminated prematurely\r\n\r\n");
                        stext6("Disconnected!");
                        break;
                    }
                    totalBytesRcvd += bytesRcvd;
                }
                stext2(totalBytesRcvd + " bytes recieved from server: " + Encoding.ASCII.GetString(byteBuffer) + "\r\n\r\n");
                stext6("Idle!");
            }
            catch (Exception excep)
            {
                stext2(excep.Message + "\r\n\r\n");
            }
            finally
            {
                stext2("Disconnected\r\n\r\n");
                stext6("Disconnected!");
                sock.Close();
            }
        }
    }
}
