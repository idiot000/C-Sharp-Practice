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
using System.Threading;
using System.Diagnostics;

namespace RemoteUtilitiesServer
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
            textBox1.Text = " Idle!";
        }

        private Socket sock = null;
        private Socket client = null;
        private bool abortFlag = false;
        private bool downloadFlag = false;

        private void sendMessage()
        {
            if (textBox3.Text.Length != 0)
            {
                client.Send(Encoding.ASCII.GetBytes(textBox3.Text + "\n"));
                DateTime Now = DateTime.Now;
                RichTextBoxExtensions.AppendText(richTextBox1, "\r\n" + "[" + Now.ToString().Substring(Now.ToString().IndexOf(" ") + 1) + "] You: " + textBox3.Text, Color.Blue);
                textBox3.Text = "";
                textBox3.Refresh();
            }
        }

        private void recvFile(string fileName,long size)
        {
            try
            {
                downloadFlag = true;
                FileStream downloadStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                client.Send(Encoding.ASCII.GetBytes("OK\n\n"));
                int bytesRcvd = 0;
                long totalBytesRcvd = 0;
                byte[] buffer = new byte[1024];
                while (true)
                {
                    bytesRcvd = client.Receive(buffer, 1024, SocketFlags.None);
                    downloadStream.Write(buffer, 0, bytesRcvd);
                    totalBytesRcvd += bytesRcvd;
                    if (totalBytesRcvd == size)
                        break;
                }
                downloadStream.Close();
                downloadFlag = false;
            }
            catch (Exception excep)
            {
                string msg = "";
                client.Send(Encoding.ASCII.GetBytes((excep.Message.EndsWith("\n") ? msg = excep.Message.TrimEnd(char.Parse("\n")) : excep.Message) + "\n\n"));
                downloadFlag = false;
            }
        }

        private void execCommand()
        {
            try
            {
                string cmd = Framer.nextFrame(client, "\n");
                if (cmd.Equals("::getdrives"))
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    foreach (DriveInfo d in drives)
                        client.Send(Encoding.ASCII.GetBytes(d.ToString() + "\n"));
                    client.Send(Encoding.ASCII.GetBytes("\n"));
                    return;
                }
                if (cmd.StartsWith("::getdirs"))
                {
                    DirectoryInfo dir = new DirectoryInfo(cmd.Substring(9));
                    DirectoryInfo[] dirs = dir.GetDirectories();
                    FileInfo[] files = dir.GetFiles();
                    bool flag = false;
                    foreach (DirectoryInfo d in dirs)
                    {
                        client.Send(Encoding.ASCII.GetBytes(d.ToString() + "\n"));
                        client.Send(Encoding.ASCII.GetBytes("File Folder\n"));
                        client.Send(Encoding.ASCII.GetBytes("Not Applicable\n"));
                        flag = true;
                    }
                    foreach (FileInfo f in files)
                    {
                        client.Send(Encoding.ASCII.GetBytes(f.ToString() + "\n"));
                        client.Send(Encoding.ASCII.GetBytes(f.Extension + " File\n"));
                        client.Send(Encoding.ASCII.GetBytes((((float)f.Length) / 1024 / 1024).ToString("0.##") + " MB\n"));
                        flag = true;
                    }
                    if (flag == false)
                        client.Send(Encoding.ASCII.GetBytes("\n\n"));
                    else
                        client.Send(Encoding.ASCII.GetBytes("\n"));
                    return;
                }
                if (cmd.Equals("::disconnect"))
                {
                    client.Send(Encoding.ASCII.GetBytes("OK\n\n"));
                    client.Close();
                    client = null;
                    return;
                }
                if (cmd.StartsWith("::download"))
                {
                    byte[] buffer = new byte[1024];
                    byte[] buff = new byte[1];
                    FileStream fstream = new FileStream(cmd.Substring(10), FileMode.Open, FileAccess.Read);
                    long totalBytesSent = 0, size = fstream.Length;
                    int bytesSent = 0;
                    client.Send(Encoding.ASCII.GetBytes(size.ToString() + "\n\n"));
                    Thread.Sleep(1000);
                    client.Receive(buff, 1, SocketFlags.None);
                    if (Encoding.ASCII.GetString(buff).Equals("1"))
                    {
                        Socket temp = null;
                        ThreadStart abortJob = delegate
                        {
                            sock.Blocking = false;
                            while (true)
                            {
                                if (abortFlag == true)
                                {
                                    abortFlag = false;
                                    break;
                                }
                                try
                                {
                                    temp = sock.Accept();
                                }
                                catch (Exception excep) { }
                                if (temp != null)
                                {
                                    abortFlag = true;
                                    temp.Close();
                                    break;
                                }
                            }
                            sock.Blocking = true;
                        };
                        new Thread(abortJob).Start();
                        while (true)
                        {
                            bytesSent = fstream.Read(buffer, 0, 1024);
                            if (abortFlag == true)
                            {
                                abortFlag = false;
                                break;
                            }
                            client.Send(buffer, bytesSent, SocketFlags.None);
                            totalBytesSent += bytesSent;
                            if (totalBytesSent == size)
                            {
                                abortFlag = true;
                                break;
                            }
                        }
                    }
                    fstream.Close();
                    return;
                }
                if (cmd.StartsWith("::exec"))
                {
                    System.Diagnostics.Process.Start(cmd.Substring(6));
                    client.Send(Encoding.ASCII.GetBytes("OK\n\n"));
                    return;
                }
                if (cmd.StartsWith("::rename"))
                {
                    string name = Framer.nextFrame(client, "\n");
                    DirectoryInfo dir = new DirectoryInfo(cmd.Substring(8, cmd.LastIndexOf(@"\") - 8));
                    string itemName = cmd.Substring(cmd.LastIndexOf(@"\") + 1);;
                    DirectoryInfo[] dirs = dir.GetDirectories();
                    bool flag = false;
                    foreach (DirectoryInfo d in dirs)
                    {
                        if (d.ToString() == itemName)
                        {
                            flag = true;
                            d.MoveTo(cmd.Substring(8, cmd.LastIndexOf(@"\") - 7) + name);
                            break;
                        }
                    }
                    if (flag != true)
                    {
                        FileInfo[] files = dir.GetFiles();
                        foreach (FileInfo f in files)
                        {
                            if (f.ToString() == itemName)
                            {
                                f.MoveTo(cmd.Substring(8, cmd.LastIndexOf(@"\") - 7) + name);
                                break;
                            }
                        }
                    }
                    client.Send(Encoding.ASCII.GetBytes("OK\n\n"));
                    return;
                }
                if (cmd.StartsWith("::delete"))
                {
                    DirectoryInfo dir = new DirectoryInfo(cmd.Substring(8, cmd.LastIndexOf(@"\") - 8));
                    string itemName = cmd.Substring(cmd.LastIndexOf(@"\") + 1);
                    DirectoryInfo[] dirs = dir.GetDirectories();
                    bool flag = false;
                    foreach (DirectoryInfo d in dirs)
                    {
                        if (d.ToString().Equals(itemName))
                        {
                            flag = true;
                            d.Delete();
                            break;
                        }
                    }
                    if (flag != true)
                    {
                        FileInfo[] files = dir.GetFiles();
                        foreach (FileInfo f in files)
                        {
                            if (f.ToString() == itemName)
                            {
                                f.Delete();
                                break;
                            }
                        }
                    }
                    client.Send(Encoding.ASCII.GetBytes("OK\n\n"));
                    return;
                }
                if (cmd.StartsWith("::chat"))
                {
                    if (this.Height != 320)
                        backgroundWorker1.ReportProgress(50, " Chat session established!");
                    backgroundWorker1.ReportProgress(100, cmd.Substring(6));
                    return;
                }
                if (cmd.Equals("::closechat"))
                {
                    client.Send(Encoding.ASCII.GetBytes("OK\n\n"));
                    backgroundWorker1.ReportProgress(0);
                    return;
                }
                if (cmd.Equals("::getsysinfo"))
                {
                    client.Send(Encoding.ASCII.GetBytes(SystemInformation.ComputerName.ToString() + "\n"));
                    client.Send(Encoding.ASCII.GetBytes(HardwareInfo.GetProcessorId() + "\n"));
                    client.Send(Encoding.ASCII.GetBytes(HardwareInfo.GetHDDSerialNo() + "\n"));
                    client.Send(Encoding.ASCII.GetBytes(SystemInformation.UserName + "\n"));
                    client.Send(Encoding.ASCII.GetBytes(System.Environment.OSVersion.ToString() + "\n"));
                    client.Send(Encoding.ASCII.GetBytes("\n"));
                    return;
                }
                if (cmd.StartsWith("::upload"))
                {
                    long size = long.Parse(Framer.nextFrame(client, "\n"));
                    Thread.Sleep(500);
                    ThreadStart fileDownload = delegate { recvFile(cmd.Substring(8), size); };
                    Thread fileThread = new Thread(fileDownload);
                    fileThread.Start();
                    Thread.Sleep(100);
                    while (downloadFlag == true) ;
                }
                if (cmd.Equals("::getprocesses"))
                {
                    Process[] processList = Process.GetProcesses();
                    foreach (Process p in processList)
                    {
                        client.Send(Encoding.ASCII.GetBytes(p.ProcessName + "\n"));
                        client.Send(Encoding.ASCII.GetBytes(p.Id.ToString() + "\n"));
                        client.Send(Encoding.ASCII.GetBytes(((long)(p.WorkingSet64 / 1024)).ToString() + " KB" + "\n"));
                    }
                    client.Send(Encoding.ASCII.GetBytes("\n"));
                }
                if (cmd.StartsWith("::terminate"))
                {
                    Process p = Process.GetProcessById(int.Parse(cmd.Substring(11)));
                    p.Kill();
                    client.Send(Encoding.ASCII.GetBytes("OK\n\n"));
                }
            }
            catch (Exception excep)
            {
                string msg = "";
                client.Send(Encoding.ASCII.GetBytes((excep.Message.EndsWith("\n") ? msg = excep.Message.TrimEnd(char.Parse("\n")) : excep.Message) + "\n\n"));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Run Server"))
            {
                if (textBox2.Text.Length == 0)
                {
                    textBox1.Text = " Missing Fields!";
                    return;
                }
                try
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(textBox2.Text));
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Bind(clientEndPoint);
                    sock.Listen(1);
                    textBox1.Text = " Running!";
                    button1.Text = "Stop";
                }
                catch (SocketException excep)
                {
                    MessageBox.Show(excep.Message);
                    textBox1.Text = " Idle";
                    return;
                }
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                abortFlag = true;
                backgroundWorker1.CancelAsync();
                if (client != null)
                {
                    client.Close();
                    client = null;
                }
                sock.Close();
                button1.Text = "Run Server";
                textBox1.Text = " Idle";
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    if (backgroundWorker1.CancellationPending)
                        break;
                    client = sock.Accept();
                    while (true)
                    {
                        if (client != null)
                            execCommand();
                        else
                            break;
                    }
                }
            }
            catch (Exception excep){}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 50)
            {
                textBox1.Text = " Chat session established!";
                textBox1.Refresh();
                this.Height = 320;
                return;
            }
            if (e.ProgressPercentage == 0)
            {
                textBox1.Text = " Running!";
                textBox1.Refresh();
                this.Height = 88;
                return;
            }
            if (e.ProgressPercentage == 100)
            {
                DateTime Now = DateTime.Now;
                RichTextBoxExtensions.AppendText(richTextBox1, "\r\n" + "[" + Now.ToString().Substring(Now.ToString().IndexOf(" ") + 1) + "] Client: " + (string)e.UserState, Color.Green);
                return;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                sendMessage();
        }
    }
}
