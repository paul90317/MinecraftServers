using MinecraftServers.headers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MinecraftServers.Forms
{
    public partial class ModsConcurrencyForm : Form
    {
        public ModsConcurrencyForm(DirectoryInfo sharedir, DirectoryInfo versionfiledir)
        {
            InitializeComponent();
            shareDir = sharedir;
            versionFileDir = versionfiledir;
            fsv = new FileServer(shareDir, 7777);
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker3.WorkerSupportsCancellation = true;
            process1.EnableRaisingEvents = true;
            process1.StartInfo.RedirectStandardOutput = true;
            process1.StartInfo.UseShellExecute = false;
            process1.StartInfo.CreateNoWindow = true;
            process1.StartInfo.FileName = "ngrok.exe";
            timer1.Interval = 500;
            timer1.Start();
        }
        DirectoryInfo shareDir;
        FileServer fsv;
        bool flag_p = true;
        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                if (!flag_p && !process1.HasExited)
                    process1.Kill();
                fsv.Stop();
                if(backgroundWorker1.IsBusy)
                    backgroundWorker1.CancelAsync();
                lock (output)
                {
                    output = "";
                    changed = true;
                }
            }
            else
            {
                lock (output)
                {
                    output = "";
                    changed = true;
                }
                int port;
                try
                {
                    port = int.Parse(textBox2.Text);
                    fsv.port = port;
                    if (port <= 0)
                    {
                        MessageBox.Show("請輸入正整數");
                        return;
                    }
                    foreach (var _interface in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        foreach (var address in _interface.GetIPProperties().UnicastAddresses)
                        {
                            if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                string s = address.Address.ToString();
                                if (s.IndexOf("192.168") == 0)
                                {
                                    lock (output)
                                    {
                                        output += s + ":" + port.ToString() + "\r\n";
                                        changed = true;
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                try
                {
                    WebRequest req = WebRequest.Create("http://api.ipify.org/");
                    using (WebResponse resp = req.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                        {
                            lock (output)
                            {
                                output += sr.ReadToEnd() + ":" + port.ToString() + "\r\n";
                                changed = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                try
                {
                    var res = MessageBox.Show("是否啟用 ngrok?", "", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes && (flag_p || process1.HasExited)) 
                    {
                        process1.StartInfo.Arguments = String.Format("tcp {0} --log=stdout", port);
                        process1.Start();
                        flag_p = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                backgroundWorker1.RunWorkerAsync();
                button1.Text = "分享中...";
                textBox2.Enabled = false;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            fsv.Start();
        }
        string output = "";
        bool changed = false;
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!flag_p)
            {
                string data = process1.StandardOutput.ReadLine();
                if (data == null || data == "") break;
                string[] sps = data.Split(' ');
                foreach(string sp in sps)
                {
                    if (sp.IndexOf("url=tcp://") == 0)
                    {
                        lock (output)
                        {
                            output += sp.Remove(0, 10) + "\r\n";
                            changed = true;
                        }
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (changed)
            {
                lock (output)
                {
                    textBox1.Text = output;
                    changed = false;
                }
            }
            if (changed3)
            {
                lock (output3)
                {
                    textBox4.Text = output3;
                    textBox4.SelectionStart = textBox4.Text.Length;
                    textBox4.ScrollToCaret();
                    changed3 = false;
                }
            }
            if (!backgroundWorker2.IsBusy)
            {
                backgroundWorker2.RunWorkerAsync();
            }
            if (backgroundWorker1.IsBusy)
            {
                button1.Text = "分享中...";
                textBox2.Enabled = false;
            }
            else
            {
                button1.Text = "分享";
                textBox2.Enabled = true;
            }
            if (backgroundWorker3.IsBusy)
            {
                button2.Text = "取消同步";
                textBox3.Enabled = false;
            }
            else
            {
                button2.Text = "同步";
                textBox3.Enabled = true;
            }
        }

        private void ModsConcurrencyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!flag_p && !process1.HasExited)
                process1.Kill();
            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();
            if (backgroundWorker2.IsBusy)
                backgroundWorker2.CancelAsync();
            if (backgroundWorker3.IsBusy)
                backgroundWorker3.CancelAsync();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker3.IsBusy)
            {
                backgroundWorker3.CancelAsync();
                return;
            }
            if (textBox3.Text == "")
            {
                MessageBox.Show("請輸入 IP 位址");
                return;
            }
            backgroundWorker3.RunWorkerAsync();
            button2.Text = "取消同步";
            textBox3.Enabled = false;
        }
        DirectoryInfo versionFileDir = null;
        string output3 = "";
        bool changed3 = false;
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            FileClient fc = null;
            try
            {
                if (textBox3.Text.IndexOf(':') != -1)
                {
                    string[] sps = textBox3.Text.Split(':');
                    fc = new FileClient(sps[0], int.Parse(sps[1]));
                }
                else
                {
                    fc = new FileClient(textBox3.Text, 7777);
                }
                lock (output3)
                {
                    output3 = "";
                    changed3 = true;
                }
                string[] files = fc.GetList();
                lock (output3)
                {
                    output3 += "已取得模組清單\r\n";
                    changed3 = true;
                }
                bool err = false;
                foreach (string f in files)
                {
                    string temp_file = versionFileDir.FullName + "/temp/" + f;
                    string glbfile = versionFileDir.FullName + "/" + f;
                    if (!File.Exists(glbfile))
                    {
                        bool fd = fc.GetFile(f, temp_file);
                        if (fd)
                        {
                            lock (output3)
                            {
                                output3 += "已下載: " + f + "\r\n";
                                changed3 = true;
                            }
                            File.Copy(temp_file, glbfile);
                        }
                        else
                        {
                            lock (output3)
                            {
                                output3 += "錯誤: 下載 " + f + " 失敗\r\n";
                                changed3 = true;
                            }
                            err = true;
                        }
                    }
                }
                if(err)
                {
                    MessageBox.Show("同步失敗: \r\n未完整下載所有模組");
                    return;
                }
                FileInfo[] del_fs = shareDir.GetFiles();
                foreach (var file in del_fs)
                {
                    if (Array.IndexOf(files, file.Name) == -1)
                    {
                        File.Delete(file.FullName);
                    }
                }
                foreach (string f in files)
                {
                    string glbfile = versionFileDir.FullName + "/" + f;
                    string fullname = shareDir.FullName + "/" + f;
                    File.Copy(glbfile, fullname, true);
                }
                MessageBox.Show("同步完成");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("同步失敗:\r\n" + ex.Message);
                return;
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                fsv.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("伺服器錯誤: " + ex.Message);
            }
           
        }

        private void ModsConcurrencyForm_Load(object sender, EventArgs e)
        {

        }
    }
}
