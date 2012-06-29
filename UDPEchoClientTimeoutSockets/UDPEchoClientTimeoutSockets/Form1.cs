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

namespace UDPEchoClientTimeoutSockets
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "Client Log";
        }
        private const int TIMEOUT = 3000;
        private const int MAXTRIES = 5;
        private void stext2(string txt)
        {
            textBox2.Text += txt;
            textBox2.SelectionStart = textBox2.Text.Length;
            textBox2.ScrollToCaret();
            textBox2.Refresh();
        }
        private void stext6(string txt)
        {
            textBox6.Text = txt;
            textBox6.Refresh();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 0 || textBox4.Text.Length == 0 || textBox5.Text.Length == 0)
            {
                stext6("Missing Fields!");
                return;
            }
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TIMEOUT);
            IPEndPoint remoteIPEndPoint = new IPEndPoint(Dns.GetHostAddresses(textBox3.Text).ElementAt(0), Int32.Parse(textBox4.Text));
            EndPoint remoteEndPoint = (EndPoint)remoteIPEndPoint;
            byte[] sendPacket = Encoding.ASCII.GetBytes(textBox5.Text);
            byte[] recvPacket = new byte[sendPacket.Length];
            int tries = 0;
            Boolean recievedResponse = false;
            do
            {
                stext6("Sending!");
                sock.SendTo(sendPacket, remoteEndPoint);
                stext2(sendPacket.Length + " bytes sent to server\r\n");
                stext6("Recieving...\r\n");
                try
                {
                    sock.ReceiveFrom(recvPacket, ref remoteEndPoint);
                    recievedResponse = true;
                }
                catch (SocketException se)
                {
                    tries++;
                    if (se.ErrorCode == 10060)
                        stext2("Timed out, " + (MAXTRIES - tries) + " tries...");
                    else
                        stext2(se.ErrorCode + ": " + se.Message + "\r\n\r\n");
                }
            }
            while ((!recievedResponse) && (tries < MAXTRIES));
            if (recievedResponse)
                stext6(recvPacket.Length + " bytes recieved from " + remoteEndPoint.ToString() + ": " + Encoding.ASCII.GetString(recvPacket, 0, recvPacket.Length));
            else
            {
                stext2("No response, giving up...\r\n\r\n");
                stext6("Idle!");
            }
            sock.Close();
        }
    }
}
