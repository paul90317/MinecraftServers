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
using MinecraftServers.Controls;
using MinecraftServers.Forms;

namespace MinecraftServers
{
    public partial class serverForm : Form
    {
        public DirectoryInfo serverDir;
        public serverForm()
        {
            InitializeComponent();
        }
        Control new_entry(string entry_name, string value)
        {
            Control fp;
            string[] vals = Program.get_entry_setting(entry_name);
            if (vals == null) return null;
            if (vals.Length > 0)
            {
                fp = new SwitchButton(entry_name, vals, value);
            }
            else
            {
                fp = new InputBox(entry_name, value);
            }
            return fp;
        }
        public void load_properties()
        {
            FileInfo profile = new FileInfo(serverDir.FullName + "/server.properties");
            if (!profile.Exists)
            {
                return;
            }
            using(StreamReader sr=new StreamReader(profile.FullName))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line[0] == '#') continue;
                    string[] sps = line.Split('=');
                    Control fp = new_entry(sps[0], sps[1]);
                    if (fp != null)
                    {
                        flowLayoutPanel6.Controls.Add(fp);
                    }
                }
                
            }
        }
        private void serverForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = serverDir.Name;
            label3.Text = serverDir.Parent.Parent.Name;
            load_properties();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string new_path = serverDir.Parent.FullName + "/" + textBox1.Text;
            if (Directory.Exists(new_path) && textBox1.Text != serverDir.Name) {
                MessageBox.Show("伺服器名稱已存在");
                return;
            }
            if (textBox1.Text != serverDir.Name)
            {
                serverDir.MoveTo(new_path);
            }
            Dictionary<string, string> mmap=new Dictionary<string, string>();
            foreach(Control p in flowLayoutPanel6.Controls)
            {
                if (p.GetType().Name == "SwitchButton") 
                {
                    SwitchButton b = (SwitchButton)p;
                    if (b.value != "")
                    {
                        mmap[b.key] = b.value;
                    }
                }
                else
                {
                    InputBox b = (InputBox)p;
                    mmap[b.key] = b.value;
                }
            }
            FileInfo file = useful.get_file_or_create(serverDir.FullName + "/server.properties");
            List<string> data = new List<string>();
            using (StreamReader sr = new StreamReader(file.FullName))
            {
                while (!sr.EndOfStream)
                {
                    data.Add(sr.ReadLine());
                }
            }
            using (StreamWriter sw = new StreamWriter(file.FullName))
            {
                foreach(var line in data)
                {
                    if (line[0] == '#') continue;
                    string[] sps = line.Split('=');
                    if (mmap.ContainsKey(sps[0]))
                    {
                        sw.WriteLine(sps[0] + "=" + mmap[sps[0]]);
                    }
                    else sw.WriteLine(line);
                }
            }
            MessageBox.Show("儲存完成");
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            Program.now = new Form1();
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定要刪除此伺服器?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Program.now = new Form1();
                try
                {
                    Directory.Delete(serverDir.FullName, true);
                }
                catch(Exception)
                {
                    MessageBox.Show("無法刪除");
                }
                Close();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Program.now = new execute(serverDir);
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SelectForm sf = new SelectForm();
            sf.Dir = useful.get_dir_or_create("./versions/" + label3.Text + "/mods");
            sf.localDir = useful.get_dir_or_create(serverDir.FullName + "/mods");
            sf.title = "模組";
            sf.Filter = "JAR|*.jar";
            sf.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SelectForm sf = new SelectForm();
            sf.Dir = useful.get_dir_or_create("./datapacks");
            sf.localDir = useful.get_dir_or_create(serverDir.FullName + "/world/datapacks");
            sf.title = "資料包";
            sf.Filter = "ZIP|*.zip";
            sf.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            properties_show_choose psc = new properties_show_choose();
            psc.serverDir = serverDir;
            psc.ShowDialog();
            Dictionary<string, string> mmap = new Dictionary<string, string>();
            foreach (var p in flowLayoutPanel6.Controls)
            {
                if (p.GetType().Name == "InputBox")
                {
                    InputBox ib = (InputBox)p;
                    mmap[ib.key] = ib.value;
                }
                else
                {
                    SwitchButton b = (SwitchButton)p;
                    if (b.value != "")
                    {
                        mmap[b.key] = b.value;
                    }
                }
            }
            while (flowLayoutPanel6.Controls.Count > 0)
            {
                flowLayoutPanel6.Controls[0].Dispose();
            }
            FileInfo profile = new FileInfo(serverDir.FullName + "/server.properties");
            if (!profile.Exists)
            {
                return;
            }
            using (StreamReader sr = new StreamReader(profile.FullName))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line[0] == '#') continue;
                    string[] sps = line.Split('=');
                    Control fp;
                    if (mmap.ContainsKey(sps[0])) fp = new_entry(sps[0], mmap[sps[0]]);
                    else fp = new_entry(sps[0], sps[1]);
                    if (fp != null)
                    {
                        flowLayoutPanel6.Controls.Add(fp);
                    }
                }

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var versdir = useful.get_dir_or_create(serverDir.Parent.Parent.FullName + "/mods/files");
            ModsConcurrencyForm mf = new ModsConcurrencyForm(useful.get_dir_or_create(serverDir.FullName + "/mods"), versdir);
            mf.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SelectForm sf = new SelectForm();
            sf.Dir = useful.get_dir_or_create("./versions/" + label3.Text + "./plugins");
            sf.localDir = useful.get_dir_or_create(serverDir.FullName + "/world/plugins");
            sf.title = "插件";
            sf.Filter = "JAR|*.jar";
            sf.ShowDialog();
        }
    }
}
