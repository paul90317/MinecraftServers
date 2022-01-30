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
using System.Text.Json;
using System.Text.Json.Serialization;
using MinecraftServers.Controls;
using System.Diagnostics;

namespace MinecraftServers.Forms
{
    public partial class Client : Form
    {
        public class Entry
        {
            public DateTime created { get; set; }
            public string gameDir { get; set; }
            public string icon { get; set; }
            public string javaArgs { get; set; }
            public DateTime lastUsed { get; set; }
            public string lastVersionId { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }
        public class Settings
        {
            public bool crashAssistance { get; set; }
            public bool enableAdvanced { get; set; }
            public bool enableAnalytics { get; set; }
            public bool enableHistorical { get; set; }
            public bool enableReleases { get; set; }
            public bool enableSnapshots { get; set; }
            public bool keepLauncherOpen { get; set; }
            public string profileSorting { get; set; }
            public bool showGameLog { get; set; }
            public bool showMenu { get; set; }
            public bool soundOn { get; set; }
        }
        public class Profiles
        {
            public Dictionary<string,Entry> profiles { get; set; }
            public Settings settings { get; set; }
            public int version { get; set; }
        }
        public Client()
        {
            InitializeComponent();
        }
        Profiles profiles;
        FileInfo lproc;
        bool open_setting()
        {
            if (File.Exists("./client.txt"))
            {
                string mc;
                using (StreamReader sr = new StreamReader("./client.txt"))
                {
                    mc = sr.ReadLine();
                }
                lproc = new FileInfo(mc);
                if (!lproc.Exists) return false;
                using(StreamReader sr =new StreamReader(lproc.FullName))
                {
                    string data = sr.ReadToEnd();
                    Console.WriteLine(data);
                    profiles = JsonSerializer.Deserialize<Profiles>(data);
                }
                return true;
            }
            else return false;
        }
        private void Client_Load(object sender, EventArgs e)
        {
            if (!open_setting())
            {
                openFileDialog1.Filter = "JSON|launcher_profiles.json";
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\.minecraft";
                openFileDialog1.FileName = "launcher_profiles.json";
                openFileDialog1.Title = "選擇 .minecraft/launcher_profiles.json";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter("./client.txt"))
                    {
                        sw.WriteLine(openFileDialog1.FileName);
                    }
                    open_setting();
                }
                else
                {
                    Close();
                    return;
                }
            }
            foreach(var p in profiles.profiles)
            {
                Entry prof = p.Value;
                MenuButton mb = new MenuButton(prof.name, prof.lastVersionId);
                mb.datas["gameDir"] = prof.gameDir;
                mb.datas["lastVersionId"] = prof.lastVersionId;
                flowLayoutPanel1.Controls.Add(mb);
                mb.add_click_eh(client_click);
            }
        }

        MenuButton mb_sel = null;
        private void button3_Click(object sender, EventArgs e)
        {
            if (mb_sel == null)
            {
                MessageBox.Show("請選擇客戶端");
                return;
            }
            string v = mb_sel.datas["lastVersionId"];
            string v_mapping = "";
            if (Program.version_mapping.ContainsKey(v))
            {
                v_mapping = Program.version_mapping[v];
            }
            if (v_mapping == "" || !Directory.Exists("./versions/" + v_mapping+"/mods"))
            {
                chooseForm cf = new chooseForm();
                List<string> ls = new List<string>();
                foreach (var p in useful.get_dir_or_create("./versions").GetDirectories())
                {
                    ls.Add(p.Name);
                }
                v_mapping = cf.ShowDialag_and_return("選擇一個版本", ls);
                if (v_mapping == null) return;
                Program.version_mapping[v] = v_mapping;
            }
            
            string vff = "./versions/" + v_mapping + "/mods/files";
            string gamedir = mb_sel.datas["gameDir"];
            if (gamedir == null)
            {
                gamedir = lproc.DirectoryName;
            }
            ModsConcurrencyForm msf = new ModsConcurrencyForm(useful.get_dir_or_create(gamedir + "/mods"), useful.get_dir_or_create(vff));
            msf.ShowDialog();
        }
        private void client_click(object sender, EventArgs e)
        {
            MenuButton mb = (MenuButton)sender;
            if (mb_sel != null)
            {
                mb_sel.foreColor = Color.White;
            }
            mb.foreColor = Color.Yellow;
            mb_sel = mb;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Program.now = new menu();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mb_sel == null)
            {
                MessageBox.Show("請選擇模組");
                return;
            }

            string v = mb_sel.datas["lastVersionId"];
            string v_mapping = "";
            if (Program.version_mapping.ContainsKey(v))
            {
                v_mapping = Program.version_mapping[v];
            }
            if (v_mapping == "" || !Directory.Exists("./versions/" + v_mapping+"/mods"))
            {
                chooseForm cf = new chooseForm();
                List<string> ls = new List<string>();
                foreach (var p in useful.get_dir_or_create("./versions").GetDirectories())
                {
                    ls.Add(p.Name);
                }
                v_mapping = cf.ShowDialag_and_return("選擇一個版本", ls);
                if (v_mapping == null) return;
                Program.version_mapping[v] = v_mapping;
            }

            string gamedir = mb_sel.datas["gameDir"];
            if (gamedir == null)
            {
                gamedir = lproc.DirectoryName;
            }

            SelectForm sf = new SelectForm();
            sf.Dir = new DirectoryInfo("./versions/" + v_mapping+"/mods");
            sf.localDir = useful.get_dir_or_create(gamedir + "/mods");
            sf.title = "模組";
            sf.Filter = "JAR|*.jar";
            sf.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (mb_sel == null)
            {
                MessageBox.Show("請選擇模組");
                return;
            }
            string gamedir = mb_sel.datas["gameDir"];
            if (gamedir == null)
            {
                gamedir = lproc.DirectoryName;
            }
            SelectForm sf = new SelectForm();
            sf.Dir = new DirectoryInfo("./resourcepacks/");
            sf.localDir = useful.get_dir_or_create(gamedir + "/resourcepacks");
            sf.title = "材質包";
            sf.Filter = "ZIP|*.zip";
            sf.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (mb_sel == null)
            {
                MessageBox.Show("請選擇模組");
                return;
            }
            string gamedir = mb_sel.datas["gameDir"];
            if (gamedir == null)
            {
                gamedir = lproc.DirectoryName;
            }
            Process.Start(gamedir);
        }
    }
}
