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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace MinecraftServers.Forms
{
    public partial class execute : Form
    {
        versionConfig vc;
        public execute(DirectoryInfo dir)
        {
            InitializeComponent();
            titleToolStripMenuItem.Text = dir.Name;
            Dir = dir;
            versionDir = dir.Parent.Parent;
            try
            {
                using (StreamReader sr = new StreamReader(versionDir.FullName + "/config.json"))
                {
                    string data = sr.ReadToEnd();
                    vc = JsonSerializer.Deserialize<versionConfig>(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                using (StreamReader sr = new StreamReader(dir.FullName + "/config.txt"))
                {
                    mem = int.Parse(sr.ReadLine());
                }
            }
            catch (Exception)
            {
                mem = 4;
            }
            toolStripTextBox1.Text = mem.ToString();
            mapInfo = dir.Name + "\r\n版本: " + versionDir.Name;
            process2.StartInfo.UseShellExecute = false;
            process2.StartInfo.FileName = "ngrok.exe";
            process2.StartInfo.CreateNoWindow = true;
            process2.StartInfo.RedirectStandardOutput = true;
            process2.EnableRaisingEvents = true;
        }
        DirectoryInfo versionDir;
        int mem;
        string mapInfo;
        DirectoryInfo Dir;
        private void 選擇JavaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JAVA|java.exe";
            FileInfo file = new FileInfo(vc.javapath);
            openFileDialog1.InitialDirectory = file.DirectoryName;
            if (openFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                vc.javapath = openFileDialog1.FileName;
            }
        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(mapInfo);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if(label1.Text=="IP address")
            {
                label1.Text = "";
                foreach (var _interface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var address in _interface.GetIPProperties().UnicastAddresses)
                    {
                        if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            string s = address.Address.ToString() + "\r\n";
                            if (s.IndexOf("192.168") == 0)
                            {
                                label1.Text += s;
                            }
                        }

                    }
                }
                WebRequest req;
                try
                {
                    req = WebRequest.Create("http://api.ipify.org/");
                    using (WebResponse res = req.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                        {
                            label1.Text += sr.ReadToEnd();
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("找不到外部網路");
                    return;
                }
            }
            else
            {
                label1.Text = "IP address";
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serverForm sf = new serverForm();
            sf.serverDir = Dir;
            Program.now = sf;
            Close();
        }
        bool flag = true;
        bool changed = false;
        private void ngrokToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!flag && !process2.HasExited) return;
            process2.StartInfo.Arguments = "tcp 25565";
            try
            {
                process2.Start();
                flag = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool flag_p1 = true;
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (input(textBox1.Text) == false)
            {
                MessageBox.Show("程序尚未啟動");
                textBox1.Text = "";
                return;
            }
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (flag_p1 || process1.HasExited)
            {
                try
                {
                    mem = int.Parse(toolStripTextBox1.Text);
                    if (mem <= 0)
                    {
                        MessageBox.Show("記憶體要是正整數");
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("記憶體要是正整數");
                    return;
                }
                using (StreamWriter sw = new StreamWriter(Dir.FullName + "/config.txt"))
                {
                    sw.WriteLine(mem.ToString());
                }
                using (StreamWriter sw = new StreamWriter(Dir.FullName + "/runServer.bat"))
                {
                    FileInfo jarf = new FileInfo(vc.jarfile);
                    string jarf_str = Dir.FullName +"/"+ jarf.Name;
                    if (jarf.Extension == ".jar")
                    {
                        sw.WriteLine("@echo off");
                        sw.WriteLine(String.Format("\"{0}\" -Xmx{2}G -Xms{2}G -jar \"{1}\" nogui", vc.javapath, jarf.Name, mem));
                    }
                    else
                    {
                        sw.WriteLine("@echo off");
                        using(StreamReader sr=new StreamReader(jarf_str))
                        {
                            while (!sr.EndOfStream)
                            {
                                string line = sr.ReadLine();
                                if (line == "pause") continue;
                                if (line.IndexOf("java") == 0)
                                {
                                    line += " nogui";
                                    line = line.Replace("java", string.Format("\"{0}\" -Xmx{1}G -Xms{1}G", vc.javapath, mem));
                                }
                                sw.WriteLine(line);
                            }
                        }
                    }
                    
                }
                process1.StartInfo.UseShellExecute = false;
                process1.StartInfo.FileName = Dir.FullName + "/runServer.bat";
                process1.StartInfo.WorkingDirectory = Dir.FullName;
                process1.StartInfo.CreateNoWindow = true;
                process1.StartInfo.RedirectStandardOutput = true;
                process1.StartInfo.RedirectStandardInput = true;
                process1.StartInfo.RedirectStandardError = true;
                process1.EnableRaisingEvents = true;
                output = "";
                process1.Start();
                flag_p1 = false;
                changed = true;
                string data;
                if (Dir.GetFiles("eula.txt").Length == 0)
                {
                    MessageBox.Show("找不到 eula.txt");
                    return;
                }
                using (StreamReader sr = new StreamReader(Dir.FullName + "/eula.txt"))
                {
                    data = sr.ReadToEnd();
                }
                using (StreamWriter sw = new StreamWriter(Dir.FullName + "/eula.txt"))
                {
                    sw.Write(data.Replace("false", "true"));
                }
                button1.Text = "stop";
                toolStripTextBox1.Enabled = false;
                選擇JavaToolStripMenuItem.Enabled = false;
            }
            else
            {
                process1.StandardInput.WriteLine("stop");
            }
        }

        private void process1_Exited(object sender, EventArgs e)
        {
            button1.Text = "start";
            toolStripTextBox1.Enabled = true;
            選擇JavaToolStripMenuItem.Enabled = true;
            lock (output)
            {
                output += "\r\n程式結束\r\n";
                changed = true;
            }
        }
        string output = "";
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!flag_p1)
            {
                string data = process1.StandardOutput.ReadLine();
                if (data.Length == 0) break;
                lock (output)
                {
                    output += data + "\r\n";
                }
                changed = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (changed)
            {
                lock (output)
                {
                    richTextBox1.Text = output;
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    changed = false;
                }
            }
            if (changed_ngrok)
            {
                lock (output_ngrok)
                {
                    label2.Text = output_ngrok;
                    changed_ngrok = false;
                }
            }
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            if (!backgroundWorker2.IsBusy)
            {
                backgroundWorker2.RunWorkerAsync();
            }
            if (!backgroundWorker3.IsBusy)
            {
                backgroundWorker3.RunWorkerAsync();
            }
        }

        private void execute_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(versionDir.FullName + "/config.json"))
            {
                string data = JsonSerializer.Serialize(vc);
                sw.Write(data);
            }
            Visible = false;
            if (!flag_p1 && !process1.HasExited)
            {
                process1.StandardInput.WriteLine("stop");
                process1.WaitForExit();
            }
               
            if (!flag && !process2.HasExited)
            {
                process2.Kill();
                process2.WaitForExit();
            }
        }

        private void execute_Load(object sender, EventArgs e)
        {
            timer1.Interval = 500;
            timer1.Start();
        }
        bool input(string cmd)
        {
            if (flag_p1 || process1.HasExited)
            {
                return false;
            }
            lock(output)
            {
                output += "輸入: " + cmd + "\r\n";
            }
            process1.StandardInput.WriteLine(cmd);
            changed = true;
            return true;
        }
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!flag_p1)
            {
                string data = process1.StandardError.ReadLine();
                if (data.Length == 0) break;
                lock (output)
                {
                    output += "錯誤: " + data + "\r\n";
                }
                changed = true;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void 檔案總管ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Dir.FullName);
        }

        string output_ngrok = "";
        bool changed_ngrok = false;
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!flag && !process2.HasExited)
            {
                string data = process2.StandardOutput.ReadLine();
                string[] sps = data.Split(' ');
                foreach(var sp in sps)
                {
                    if (sp.IndexOf("url=tcp://") == 0)
                    {
                        output_ngrok += sp.Remove(0, 10) + "\r\n";
                        changed_ngrok = true;
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (!flag && !process2.HasExited)
            {
                process2.Kill();
            }
            else
            {
                try
                {
                    process2.StartInfo.Arguments = String.Format("tcp 25565 --log=stdout");
                    lock (output_ngrok)
                    {
                        output_ngrok = "";
                        changed_ngrok = true;
                    }
                    process2.Start();
                    flag = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void process2_Exited(object sender, EventArgs e)
        {
            lock (output_ngrok)
            {
                output_ngrok = "ngrok";
                changed_ngrok = true;
            }
        }
    }
}
