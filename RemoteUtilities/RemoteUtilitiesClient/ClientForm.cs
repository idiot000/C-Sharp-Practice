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
using System.Threading;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace RemoteUtilitiesClient
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
            textBox3.Text = " Idle";
            TextWriter tw = new StreamWriter("settings.ini", true);
            tw.Close();
            TextReader tr = new StreamReader("settings.ini");
            String line = null;
            line = tr.ReadLine();
            tr.Close();
            if (line == null)
            {
                tw = new StreamWriter("settings.ini", false);
                tw.Write("current");
                tw.Close();
            }
        }

        private Socket server = null;
        private string currPath = "";
        private bool cancelDownload = false;

        private void disconnect()
        {
            server.Close();
            Thread.Sleep(1000);
            button1.Text = "Connect";
            disablePanel();
            disableFileManager();
            textBox3.Text = " Disconnected!";
        }

        private void sendMessage()
        {
            if (textBox5.Text.Length != 0)
            {
                server.Send(Encoding.ASCII.GetBytes("::chat" + textBox5.Text + "\n"));
                DateTime Now = DateTime.Now;
                RichTextBoxExtensions.AppendText(richTextBox1, "\r\n" + "[" + Now.ToString().Substring(Now.ToString().IndexOf(" ") + 1) + "] You: " + textBox5.Text, Color.Blue);
                textBox5.Text = "";
                textBox5.Refresh();
            }
        }

        private void enablePanel()
        {
            button2.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button10.Enabled = true;
        }

        private void disablePanel()
        {
            button2.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button10.Enabled = false;
        }

        private void enableFileManager()
        {
            button3.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            button8.Visible = true;
            button9.Visible = true;
            button8.Enabled = false;
            textBox4.Visible = true;
            listView1.Visible = true;
            comboBox1.Visible = true;
            label3.Visible = true;
            listView1.Items.Clear();
            comboBox1.Items.Clear();
            comboBox1.Enabled = false;
            this.Height = 458;
            disablePanel();
            textBox3.Text = " File manager opened!";
            textBox3.Refresh();
            textBox4.Text = "";
        }

        private void disableFileManager()
        {
            this.Height = 150;
            button3.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            button8.Visible = false;
            button9.Visible = false;
            button8.Enabled = false;
            button3.Enabled = false;
            button7.Enabled = false;
            textBox4.Visible = false;
            listView1.Visible = false;
            comboBox1.Visible = false;
            label3.Visible = false;
            enablePanel();
            textBox3.Text = " File manager closed!";
            textBox3.Refresh();
        }

        private void enableProcessManager()
        {
            listView2.Visible = true;
            button13.Visible = true;
            button14.Visible = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button10.Enabled = false;
            this.Height = 458;
            button13.Refresh();
            button14.Refresh();
            listView2.Refresh();
        }
        private void disableProcessManager()
        {
            this.Height = 150;
            listView2.Visible = false;
            button13.Visible = false;
            button14.Visible = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button10.Enabled = true;
        }

        private void enableChat()
        {
            richTextBox1.Visible = true;
            textBox5.Visible = true;
            button11.Visible = true;
            button12.Visible = true;
            this.Height = 458;
            textBox3.Text = " Chat session established!";
            textBox3.Refresh();
        }

        private void disableChat()
        {
            this.Height = 150;
            richTextBox1.Visible = false;
            textBox5.Visible = false;
            button11.Visible = false;
            button12.Visible = false;
            textBox3.Text = " Chat closed!";
            textBox3.Refresh();
        }

        private void populateFileManager(List<string> dirs)
        {
            listView1.Items.Clear();
            ListViewItem item = null;
            ListViewItem.ListViewSubItem sItem = null;
            int i = 0;
            foreach (string s in dirs)
            {
                if (i % 3 == 0)
                {
                    item = new ListViewItem();
                    item.Text = s.ToString();
                }
                else
                {
                    sItem = new ListViewItem.ListViewSubItem();
                    sItem.Text = s.ToString();
                    item.SubItems.Add(sItem);
                    if (i % 3 == 2)
                    {
                        listView1.Items.Add(item);
                        listView1.Refresh();
                    }
                }
                i++;
            }
        }

        private void populateProcessManager()
        {
            textBox3.Text = " Retrieving processes...";
            textBox3.Refresh();
            List<string> processes = Framer.sendCommand(server, "::getprocesses\n", "\n");
            ListViewItem process = null;
            int i = 0;
            foreach (string p in processes)
            {
                if (i % 3 == 0)
                {
                    process = new ListViewItem();
                    process.Text = p;
                }
                else
                {
                    ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                    subItem.Text = p;
                    process.SubItems.Add(subItem);
                    if (i % 3 == 2)
                    {
                        listView2.Items.Add(process);
                        listView2.Refresh();
                    }
                }
                i++;
            }
            textBox3.Text = " Processes retrieved successfully!";
            textBox3.Refresh();
        }

        private string selectedItemType()
        {
            ListView.SelectedListViewItemCollection selectedItem = listView1.SelectedItems;
            ListViewItem.ListViewSubItemCollection sItems = selectedItem[0].SubItems;
            return sItems[1].Text;
        }

        private void getListing()
        {
            if (!selectedItemType().Equals("File Folder"))
                return;
            List<string> dirs = null;
            ListView.SelectedListViewItemCollection selectedItem = listView1.SelectedItems;
            foreach (ListViewItem item in selectedItem)
            {
                textBox3.Text = " Getting directory listing...";
                textBox3.Refresh();
                if (!currPath.EndsWith(@"\"))
                    currPath += @"\";
                currPath += item.Text;
                dirs = Framer.sendCommand(server, "::getdirs" + currPath + "\n", "\n");
            }
            populateFileManager(dirs);
            textBox4.Text = currPath;
            textBox4.Refresh();
            textBox3.Text = " Listing retrieved successufully!";
            button8.Enabled = true;
        }

        private void recvFile(long size,string fileName, string downloadDir)
        {
            try
            {
                FileStream fstream = new FileStream(downloadDir + fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                byte[] buffer = new byte[1024];
                long totalBytesRcvd = 0;
                int bytesRcvd = 0;
                while (true)
                {
                    if (cancelDownload == true)
                    {
                        Thread.Sleep(500);
                        if (server.Available > 0)
                        {
                            byte[] flushBuffer = new byte[server.Available];
                            bytesRcvd = server.Receive(flushBuffer, server.Available, SocketFlags.None);
                        }
                        fstream.Close();
                        cancelDownload = false;
                        break;
                    }
                    bytesRcvd = server.Receive(buffer, ((size - totalBytesRcvd) < 1024 ? (int)(size - totalBytesRcvd) : 1024), SocketFlags.None);
                    fstream.Write(buffer, 0, bytesRcvd);
                    totalBytesRcvd += bytesRcvd;
                    if (totalBytesRcvd == size)
                        break;
                }
                fstream.Close();
            }
            catch (Exception excep)
            {
                backgroundWorker1.ReportProgress(100, excep.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.Text.Equals("Connect"))
                {
                    textBox3.Text = " Connecting...";
                    textBox3.Refresh();
                    try
                    {
                        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        server.Connect(Dns.GetHostAddresses(textBox1.Text), int.Parse(textBox2.Text));
                        if (server.Poll(1000, SelectMode.SelectWrite))
                        {
                            textBox3.Text = " Connected!";
                            button1.Text = "Disconnect";
                            enablePanel();
                        }
                    }
                    catch (Exception excep)
                    {
                        MessageBox.Show(excep.Message);
                        textBox3.Text = " Idle!";
                    }
                }
                else
                {
                    textBox3.Text = " Disconnecting...";
                    textBox3.Refresh();
                    if (Framer.sendCommand(server, "::disconnect\n", "\n").ElementAt(0).ToString().Equals("OK"))
                        disconnect();
                }
            }
            catch (Exception excep)
            {
                textBox3.Text = " " + excep.Message;
                textBox3.Refresh();
            }
        }
       
        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text = " Getting Drives...";
            textBox3.Refresh();
            comboBox1.Items.Clear();
            if (server.Poll(100, SelectMode.SelectRead))
            {
                disconnect();
                return;
            }
            List<string> drives = Framer.sendCommand(server, "::getdrives\n", "\n");
            foreach (String s in drives)
                comboBox1.Items.Add(s.ToString());
            comboBox1.Enabled = true;
            comboBox1.Text = "Select Drive";
            comboBox1.Refresh();
            button7.Enabled = false;
            textBox3.Text = " Drives retrieved successfully!";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox3.Text = " Getting directory listing...";
            textBox3.Refresh();
            if (server.Poll(100, SelectMode.SelectRead))
            {
                disconnect();
                return;
            }
            List<string> dirs = Framer.sendCommand(server, "::getdirs"+comboBox1.SelectedItem.ToString()+"\n", "\n");
            populateFileManager(dirs);
            currPath = comboBox1.SelectedItem.ToString();
            textBox4.Text = currPath;
            textBox4.Refresh();
            textBox3.Text = " Listing retrieved successufully!";
            button3.Enabled = true;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            button7.Enabled = true;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            getListing();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox3.Text = " Getting directory listing...";
            textBox3.Refresh();
            if(currPath.LastIndexOf(@"\")!=2)
                currPath = currPath.Remove(currPath.LastIndexOf(@"\"));
            else
                currPath = currPath.Remove(currPath.LastIndexOf(@"\")+1);
            List<string> dirs = null;
            dirs = Framer.sendCommand(server, "::getdirs" + currPath + "\n", "\n");
            populateFileManager(dirs);
            textBox4.Text = currPath;
            textBox4.Refresh();
            if (currPath.Length == 3)
                button8.Enabled = false;
            textBox3.Text = " Listing retrieved successfully!";
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point current = Cursor.Position;
                current = listView1.PointToClient(current);
                if (selectedItemType().Equals("File Folder"))
                {
                    contextMenuStrip1.Items[0].Enabled = true;
                    contextMenuStrip1.Items[1].Enabled = false;
                    contextMenuStrip1.Items[2].Enabled = false;
                    contextMenuStrip1.Show(listView1, current);
                    return;
                }
                if (selectedItemType().StartsWith(".exe"))
                {
                    contextMenuStrip1.Items[0].Enabled = false;
                    contextMenuStrip1.Items[1].Enabled = true;
                    contextMenuStrip1.Items[2].Enabled = true;
                    contextMenuStrip1.Show(listView1, current);
                    return;
                }
                else
                {
                    contextMenuStrip1.Items[0].Enabled = false;
                    contextMenuStrip1.Items[1].Enabled = true;
                    contextMenuStrip1.Items[2].Enabled = false;
                    contextMenuStrip1.Show(listView1, current);
                    return;
                }
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            contextMenuStrip1.Hide();
            string filePath = "";
            if (currPath.Length == 2)
                filePath = currPath + listView1.SelectedItems[0].Text;
            else
                filePath = currPath + @"\" + listView1.SelectedItems[0].Text;
            try
            {
                if (e.ClickedItem == contextMenuStrip1.Items[0])
                {
                    getListing();
                    return;
                }
                if (e.ClickedItem == contextMenuStrip1.Items[1])
                {
                    long size = 0;
                    List<string> backArg = new List<string>();
                    string fileName = listView1.SelectedItems[0].Text;
                    textBox3.Text = " Initiating download...";
                    textBox3.Refresh();
                    button1.Enabled = false;
                    button6.Enabled = false;
                    button7.Enabled = false;
                    button8.Enabled = false;
                    button3.Enabled = false;
                    button9.Text = "Terminate Download";
                    progressBar1.Value = 0;
                    progressBar1.Visible = true;
                    string downloadDir = null, line = null;
                    TextReader tr = new StreamReader("settings.ini");
                    line = tr.ReadLine();
                    tr.Close();
                    if (line != null)
                    {
                        if (line.StartsWith("current"))
                        {
                            downloadDir = Directory.GetCurrentDirectory();
                            if (!downloadDir.EndsWith(@"\"))
                                downloadDir += @"\";
                        }
                        else
                            if (!line.EndsWith(@"\"))
                            {
                                line += @"\";
                                downloadDir = line;
                            }
                    }
                    else
                    {
                        downloadDir = Directory.GetCurrentDirectory();
                        if (!downloadDir.EndsWith(@"\"))
                            downloadDir += @"\";
                    }
                    size = long.Parse(Framer.sendCommand(server, "::download" + filePath + "\n", "\n").ElementAt(0));
                    backArg.Add(fileName);
                    backArg.Add(size.ToString());
                    backArg.Add(downloadDir);
                    server.Send(Encoding.ASCII.GetBytes("1"));
                    ThreadStart downloadJob = delegate { recvFile(size, fileName, downloadDir); };
                    Thread downloadThread = new Thread(downloadJob);
                    downloadThread.Start();
                    backgroundWorker1.RunWorkerAsync(backArg);
                    return;
                }
                if (e.ClickedItem == contextMenuStrip1.Items[2])
                {
                    string result = Framer.sendCommand(server, "::exec" + filePath + "\n", "\n").ElementAt(0);
                    if (result.Equals("OK"))
                    {
                        textBox3.Text = " File executed successfully!";
                        textBox3.Refresh();
                    }
                    else
                    {
                        textBox3.Text = " " + result;
                        textBox3.Refresh();
                    }
                    return;
                }
                if (e.ClickedItem == contextMenuStrip1.Items[3])
                {
                    string item = listView1.SelectedItems[0].Text;
                    char[] invalidFilenameChars = Path.GetInvalidFileNameChars();
                    string name = Interaction.InputBox("Rename file:", "", filePath.Substring(filePath.LastIndexOf(@"\") + 1));
                    if (name.Equals(item) || name.Length < 1)
                        return;
                    foreach (char c in invalidFilenameChars)
                    {
                        if (name.Contains(c))
                        {
                            textBox3.Text = " Name contains invalid characters!";
                            textBox3.Refresh();
                            return;
                        }
                    }
                    textBox3.Text = " Renaming...";
                    textBox3.Refresh();
                    string result = Framer.sendCommand(server, "::rename" + filePath + "\n" + name + "\n", "\n").ElementAt(0);
                    if (result.Equals("OK"))
                    {
                        List<string> dirs = Framer.sendCommand(server, "::getdirs" + currPath + "\n", "\n");
                        populateFileManager(dirs);
                        textBox3.Text = " File renamed successfully!";
                        textBox3.Refresh();
                    }
                    else
                    {
                        textBox3.Text = " " + result;
                        textBox3.Refresh();
                    }
                    return;
                }
                if (e.ClickedItem == contextMenuStrip1.Items[4])
                {
                    textBox3.Text = " Deleting...";
                    textBox3.Refresh();
                    string result = Framer.sendCommand(server, "::delete" + filePath + "\n", "\n").ElementAt(0);
                    if (result.Equals("OK"))
                    {
                        List<string> dirs = Framer.sendCommand(server, "::getdirs" + currPath + "\n", "\n");
                        populateFileManager(dirs);
                        textBox3.Text = " File deleted successfully!";
                        textBox3.Refresh();
                    }
                    else
                    {
                        textBox3.Text = " " + result;
                        textBox3.Refresh();
                    }
                    return;
                }
            }
            catch (Exception excep)
            {
                textBox3.Text = " " + excep.Message;
                textBox3.Refresh();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            long prevSize = 0, size = long.Parse(((List<string>)e.Argument).ElementAt(1));
            string fileName = ((List<string>)e.Argument).ElementAt(0);
            string downloadDir = ((List<string>)e.Argument).ElementAt(2);
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan ts;
            string report = "";
            Thread.Sleep(100);
            stopWatch.Start();
            try
            {
                FileStream f = new FileStream(downloadDir + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                while (true)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        f.Close();
                        FileInfo file = new FileInfo(downloadDir + fileName);
                        stopWatch.Restart();
                        while (stopWatch.ElapsedMilliseconds <= 3000) ;
                        file.Delete();
                        stopWatch.Stop();
                        backgroundWorker1.ReportProgress(100, " Download terminated!");
                        break;
                    }
                    ts = stopWatch.Elapsed;
                    if (ts.Milliseconds >= 500)
                    {
                        stopWatch.Restart();
                        report = " Downloading at " + ((f.Length - prevSize) / 1024 * 2).ToString() + "KB/s" + "  |  " + (f.Length / 1024).ToString() + "/" + (size / 1024).ToString() + " KiloBytes  |  " + ((double)f.Length / size * 100).ToString("#") + "% completed!";
                        int percentage = (int)((double)f.Length / size * 100);
                        if (f.Length == size)
                        {
                            stopWatch.Stop();
                            backgroundWorker1.ReportProgress(100, " Download completed successfully!");
                            break;
                        }
                        backgroundWorker1.ReportProgress(percentage, report);
                        prevSize = f.Length;
                    }
                }
                f.Close();
            }
            catch (Exception excep)
            {
                backgroundWorker1.ReportProgress(100, excep.Message);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            textBox3.Text = (string)e.UserState;
            textBox3.Refresh();
            if (e.ProgressPercentage == 100)
            {
                progressBar1.Visible = false;
                button1.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                button8.Enabled = true;
                button3.Enabled = true;
                button9.Text = "Close";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (button9.Text == "Terminate Download")
            {
                textBox3.Text = " Terminating download...";
                textBox3.Refresh();
                Socket temp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                temp.Connect(Dns.GetHostAddresses(textBox1.Text), int.Parse(textBox2.Text));
                cancelDownload = true;
                backgroundWorker1.CancelAsync();
                return;
            }
            else
            {
                if (button1.Text != "Connect")
                    enablePanel();
                disableFileManager();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            enableFileManager();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
            enableChat();
            disablePanel();
            button1.Enabled = false;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            string msg="";
            while (true)
            {
                if (backgroundWorker2.CancellationPending)
                    break;
                if (server.Available > 0)
                {
                    msg = Framer.nextFrame(server, "\n");
                    backgroundWorker2.ReportProgress(100, msg);
                }
            }
        }
        
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DateTime Now=DateTime.Now;
            RichTextBoxExtensions.AppendText(richTextBox1, "\r\n" + "[" + Now.ToString().Substring(Now.ToString().IndexOf(" ") + 1) + "] Server: " + (string)e.UserState, Color.Green);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            backgroundWorker2.CancelAsync();
            Thread.Sleep(500);
            string result = Framer.sendCommand(server, "::closechat\n", "\n").ElementAt(0);
            if (result == "OK")
            {
                textBox3.Text = " Chat closed!";
                textBox3.Refresh();
                button1.Enabled = true;
            }
            else
            {
                textBox3.Text = " " + result;
                textBox3.Refresh();
            }
            disableChat();
            enablePanel();
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                sendMessage();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            List<string> sysinfo = new List<string>();
            sysinfo = Framer.sendCommand(server, "::getsysinfo\n", "\n");
            this.Enabled = false;
            new SystemInfo(this, sysinfo).Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream uploadStream = null;
                if ((uploadStream = (FileStream)openFileDialog1.OpenFile()) != null)
                {
                    textBox3.Text = " Initiating upload...";
                    textBox3.Refresh();
                    button1.Enabled = false;
                    button6.Enabled = false;
                    button7.Enabled = false;
                    button8.Enabled = false;
                    progressBar1.Value = 0;
                    progressBar1.Visible = true;
                    backgroundWorker3.RunWorkerAsync(uploadStream);
                }
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            FileStream uploadStream=(FileStream)e.Argument;
            string fileName;
            if (currPath.EndsWith("\\"))
                fileName = uploadStream.Name.Substring(uploadStream.Name.LastIndexOf(@"\") + 1);
            else
                fileName = uploadStream.Name.Substring(uploadStream.Name.LastIndexOf(@"\"));
            string result = Framer.sendCommand(server, "::upload" + currPath + fileName + "\n" + uploadStream.Length + "\n", "\n").ElementAt(0);
            if (result.Equals("OK"))
            {
                long size = uploadStream.Length, prevSent = 0;
                int bytesSent = 0, totalBytesSent = 0;
                Byte[] buffer = new Byte[1024];
                string report = "";
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                while (true)
                {
                    bytesSent = uploadStream.Read(buffer, 0, 1024);
                    server.Send(buffer, bytesSent, SocketFlags.None);
                    totalBytesSent += bytesSent;
                    if (stopWatch.ElapsedMilliseconds >= 500)
                    {
                        stopWatch.Restart();
                        report = " Uploading at " + ((totalBytesSent - prevSent) / 1024 * 2).ToString() + "KB/s" + "  |  " + (totalBytesSent / 1024).ToString() + "/" + (size / 1024).ToString() + " KiloBytes  |  " + ((double)totalBytesSent / size * 100).ToString("#") + "% completed!";
                        backgroundWorker3.ReportProgress((int)((double)totalBytesSent / size * 100), report);
                        prevSent = totalBytesSent;
                    }
                    if (totalBytesSent == size)
                    {
                        backgroundWorker3.ReportProgress(100,"Upload completed!");
                        uploadStream.Close();
                        break;
                    }
                }
            }
            else
            {
                backgroundWorker3.ReportProgress(100, result);
            }
        }

        private void backgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Refresh();
            if (e.ProgressPercentage != 100)
            {
                textBox3.Text = (string)e.UserState;
                textBox3.Refresh();
            }
            else
            {
                if (((string)e.UserState).Equals("Upload completed!"))
                {
                    List<string> dirs = Framer.sendCommand(server, "::getdirs" + currPath + "\n", "\n");
                    populateFileManager(dirs);
                }
                textBox3.Text = " " + (string)e.UserState;
                textBox3.Refresh();
                button1.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                if (currPath.Length > 3)
                    button8.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            enableProcessManager();
            populateProcessManager();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            disableProcessManager();
            textBox3.Text = " Process Manager closed!";
            textBox3.Refresh();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            populateProcessManager();
        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point current = Cursor.Position;
                current = listView2.PointToClient(current);
                contextMenuStrip2.Show(listView2, current);
            }
        }

        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            contextMenuStrip2.Hide();
            if (e.ClickedItem == contextMenuStrip2.Items[0])
            {
                textBox3.Text = " Terminating process...";
                textBox3.Refresh();
                String result = Framer.sendCommand(server, "::terminate" + listView2.SelectedItems[0].SubItems[1].Text + "\n", "\n").ElementAt(0);
                if (result.Equals("OK"))
                {
                    textBox3.Text = " Process terminated successfully!";
                    textBox3.Refresh();
                    listView2.Items.Clear();
                    populateProcessManager();
                }
                else
                {
                    textBox3.Text = result;
                    textBox3.Refresh();
                }
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            new About(this).Show();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            new Settings(this).Show();
        }
    }
}

