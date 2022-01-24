using MinecraftServers.Controls;
using MinecraftServers.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftServers
{
    
    public partial class SelectForm : Form
    {
        class tag_button : Label 
        {
            bool selected = false;
            ControlCollection cc_sel, cc_unsel;
            List<CountButton> cbs = new List<CountButton>();
            public void sel()
            {
                selected = true;
                BackColor = Color.Green;
                if (cc_sel.IndexOf(this) == -1) 
                {
                    cc_unsel.Remove(this);
                    cc_sel.Add(this);
                }
                foreach(var cb in cbs)
                {
                    cb.showing();
                }
            }
            public void unsel()
            {
                selected = false;
                BackColor = Color.Gray;
                if (cc_unsel.IndexOf(this) == -1)
                {
                    cc_sel.Remove(this);
                    cc_unsel.Add(this);
                }
                foreach (var cb in cbs)
                {
                    cb.hiding();
                }
            }
            public void switch_sel()
            {
                if (selected)
                {
                    unsel();
                }
                else
                {
                    sel();
                }
            }
            public void on_click(object sender, EventArgs e)
            {
                switch_sel();
            }
            public void Add(CountButton cb)
            {
                cbs.Add(cb);
            }
            public tag_button(string t, ControlCollection cs, ControlCollection cus)
            {
                cc_sel = cs;
                cc_unsel = cus;
                cus.Add(this);
                BackColor = Color.Gray;
                ForeColor = Color.White;
                AutoSize = true;
                Click += on_click;
                Text = t;
            }
            public bool get_sel()
            {
                return selected;
            }
        }
        public SelectForm()
        {
            InitializeComponent();
        }
        
        
        Dictionary<string, tag_button> tags_map = new Dictionary<string, tag_button>();
        Dictionary<string, CountButton> cbs_map = new Dictionary<string, CountButton>();
        public DirectoryInfo Dir;
        public DirectoryInfo localDir;
        void save_files()
        {
            foreach(var p in file_mods_cnt)
            {
                if (p.Value > 0)
                {
                    if (localDir.GetFiles(p.Key).Length == 0)
                    {
                        File.Copy(Dir.FullName + "/files/" + p.Key, localDir.FullName + "/" + p.Key);
                    }
                }
                else
                {
                    if (localDir.GetFiles(p.Key).Length > 0)
                    {
                        File.Delete(localDir.FullName + "/" + p.Key);
                    }
                }
            }
        }
        Dictionary<string, List<string>> mod_to_files = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> file_to_mods = new Dictionary<string, List<string>>();
        Dictionary<string, int> mod_files_cnt = new Dictionary<string, int>();
        Dictionary<string, int> file_mods_cnt = new Dictionary<string, int>();
        private void SelectForm_Load(object sender, EventArgs e)
        {
            DirectoryInfo dirt = useful.get_dir_or_create(Dir.FullName + "/datas");
            button1.Text += label3.Text;
            string errs = "";
            foreach (var m in dirt.GetFiles())  
            {
                string modname = m.Name.Replace(".txt", "");
                using (StreamReader sr = new StreamReader(m.FullName)) 
                {
                    string[] tags = sr.ReadLine().Split(' ');
                    if (sr.EndOfStream)
                    {
                        errs += ("模組 " + modname + " 不含檔案\r\n");
                        continue;
                    }
                    //模組集檔案
                    mod_to_files[modname] = new List<string>();
                    while (!sr.EndOfStream)
                    {
                        string filename = sr.ReadLine();
                        if (Dir.GetDirectories("files").Length == 0 && Dir.GetDirectories("files")[0].GetFiles(filename)[0].Length == 0) 
                        {
                            errs += ("模組 " + modname + " 遺失檔案 " + filename + "\r\n");
                            continue;
                        }
                        mod_to_files[modname].Add(filename);
                        if (!file_to_mods.ContainsKey(filename)) file_to_mods[filename] = new List<string>();
                        file_to_mods[filename].Add(modname);
                        if (!mod_files_cnt.ContainsKey(modname)) mod_files_cnt[modname] = 0;
                        mod_files_cnt[modname]++;
                    }
                    //模組 tags
                    CountButton cb = new CountButton(modname, tags, flowLayoutPanel9.Controls, flowLayoutPanel8.Controls, mod_to_files[modname], file_mods_cnt);//模組按鈕
                    cbs_map[modname] = cb;
                    foreach (var t in tags) 
                    {
                        if (!tags_map.ContainsKey(t))
                        {
                            tags_map[t] = new tag_button(t, flowLayoutPanel4.Controls, flowLayoutPanel3.Controls);
                        }
                        tags_map[t].Add(cb);
                    }
                }
            }
            //local 模組
            foreach (var f in localDir.GetFiles())
            {
                string filename = f.Name;
                file_mods_cnt[filename] = 0;
                if (!file_to_mods.ContainsKey(filename))
                {
                    continue;
                }
                foreach (string modname in file_to_mods[filename])
                {
                    mod_files_cnt[modname]--;
                }
            }
            foreach (var p in mod_files_cnt)
            {
                if (p.Value == 0)
                {
                    cbs_map[p.Key].sel();
                }
            }
            List<string> unknown_files = new List<string>();
            foreach(var p in file_mods_cnt)
            {
                if (p.Value == 0)
                {
                    unknown_files.Add(p.Key);
                }
            }
            if (unknown_files.Count > 0)
            {
                string modname = "未知";
                string[] tags = new string[] { "未知" };
                CountButton cb = new CountButton(modname, tags, flowLayoutPanel9.Controls, flowLayoutPanel8.Controls, unknown_files, file_mods_cnt);
                if (!tags_map.ContainsKey("未知"))
                {
                    tags_map["未知"] = new tag_button("未知", flowLayoutPanel4.Controls, flowLayoutPanel3.Controls);
                }
                tags_map["未知"].Add(cb);
                cb.sel();
            }
            if (errs.Length > 0)
            {
                MessageBox.Show(errs);
            }
        }
        public string title
        {
            set
            {
                label3.Text = value;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            save_files();
            MessageBox.Show("已儲存");
            Close();
        }
        public string Filter;
        private void button1_Click(object sender, EventArgs e)
        {
            ImportForm ifo = new ImportForm(Dir, Filter);
            ifo.what = label3.Text;
            ifo.ShowDialog();
            if (ifo.tags.Count == 0)
            {
                return;
            }
            string[] arr = new string[ifo.tags.Count];
            for(int i = 0; i < arr.Length; i++)
            {
                arr[i] = ifo.tags[i];
            }
            CountButton cb = new CountButton(ifo.packname, arr, flowLayoutPanel9.Controls, flowLayoutPanel8.Controls, ifo.files, file_mods_cnt);
            foreach(string t in arr)
            {
                if (!tags_map.ContainsKey(t))
                {
                    tags_map[t] = new tag_button(t, flowLayoutPanel4.Controls, flowLayoutPanel3.Controls);
                }
                tags_map[t].Add(cb);
                if (tags_map[t].get_sel())
                {
                    cb.showing();
                }
            }
            ifo.Dispose();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
