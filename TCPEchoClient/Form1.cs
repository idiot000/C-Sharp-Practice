using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;

namespace TCPEchoClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox2.Text = "Client Log";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            TcpClient client=null;
            NetworkStream netStream=null;
            try
            {
                if (textBox3.Text.Length==0||textBox6.Text.Length==0)
                {
                    textBox4.Text = "IP/Port field empty!";
                    return;
                }
                textBox1.Text += "Connecting...\r\n";
                textBox4.Text = "Connecting...";
                textBox4.Refresh();
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                textBox1.Refresh();
                client = new TcpClient(textBox3.Text, Int32.Parse(textBox6.Text));
                textBox1.Text += "Connected!\r\n\r\n";
                textBox4.Text = "Connected!";
                textBox4.Refresh();
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                textBox1.Refresh();
                netStream = client.GetStream();
                if (textBox5.Text.Length==0)
                {
                    textBox1.Text += "Disconnected.\r\n\r\n";
                    textBox4.Text = "Disconnected!";
                    textBox4.Refresh();
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox1.ScrollToCaret();
                    textBox1.Refresh();
                    return;
                }
                byte[] byteBuffer = Encoding.ASCII.GetBytes(textBox5.Text);
                netStream.Write(byteBuffer, 0, byteBuffer.Length);
                textBox1.Text += byteBuffer.Length.ToString() + " bytes sent to server!\r\n\r\n";
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                textBox1.Refresh();
                int totalBytesRecvd = 0;
                int bytesRecvd = 0;
                while (bytesRecvd < byteBuffer.Length)
                {
                    if ((bytesRecvd = netStream.Read(byteBuffer, totalBytesRecvd, byteBuffer.Length - totalBytesRecvd)) == 0)
                    {
                        textBox1.Text += "Connection closed prematurely!\r\nDisconnected.\r\n\r\n";
                        textBox4.Text = "Disconnected!";
                        textBox4.Refresh();
                        textBox1.SelectionStart = textBox1.Text.Length;
                        textBox1.ScrollToCaret();
                        textBox1.Refresh();
                        break;
                    }
                    totalBytesRecvd += bytesRecvd;
                }
                textBox1.Text += "Server Response: " + Encoding.ASCII.GetString(byteBuffer, 0, totalBytesRecvd) + "\r\n";
                textBox1.Text += totalBytesRecvd.ToString() + " bytes recieved!\r\nDisconnected.\r\n";
                textBox4.Text = "Disconnected!";
                textBox4.Refresh();
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                textBox1.Refresh();
            }
            catch (Exception excep)
            {
                textBox1.Text += excep.Message + "\r\n\r\n";
                textBox4.Text = "Disconnected!";
                textBox4.Refresh();
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                textBox1.Refresh();
            }
            finally
            {
                if (client != null)
                {
                    netStream.Close();
                    client.Close();
                }
            }
        }
    }
}
