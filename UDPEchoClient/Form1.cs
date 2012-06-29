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

namespace UDPEchoClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "Client Log";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 0 || textBox4.Text.Length == 0 || textBox5.Text.Length == 0)
            {
                textBox6.Text = "Missing Fields!";
                textBox6.Refresh();
                return;
            }
            byte[] sendPacket = Encoding.ASCII.GetBytes(textBox5.Text);
            UdpClient client = new UdpClient();
            try
            {
                client.Send(sendPacket, sendPacket.Length, textBox3.Text, Int32.Parse(textBox4.Text));
                textBox2.Text += sendPacket.Length + " bytes sent to " + textBox3.Text + "\r\n\r\n";
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.ScrollToCaret();
                textBox6.Text = "Sent!";
                textBox6.Refresh();
                IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] recvPacket = client.Receive(ref remoteIPEndPoint);
                textBox2.Text += "Recieved: " + Encoding.ASCII.GetString(recvPacket) + " (" + recvPacket.Length + " bytes) from " + remoteIPEndPoint.Address + "\r\n\r\n";
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.ScrollToCaret();
                textBox2.Refresh();
                textBox6.Text = "Recieved!";
            }
            catch (SocketException se)
            {
                textBox2.Text += se.ErrorCode + ": " + se.Message + " \r\n\r\n";
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.ScrollToCaret();
                textBox2.Refresh();
                textBox6.Text = "Error!";
                textBox6.Refresh();
            }
        }
    }
}
