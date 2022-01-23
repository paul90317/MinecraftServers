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

namespace MinecraftServers.Forms
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
        }
        public ImportForm(DirectoryInfo dir, string filter)
        {
            InitializeComponent();
            Dir = dir;
            openFileDialog1.Filter = filter;
            openFileDialog1.Multiselect = true;
        }
        DirectoryInfo Dir;

        private void flowLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void tag_on_click(object sender, EventArgs e)
        {
            tag_button tb = (tag_button)sender;
            if (tb.selected)
            {
                tb.selected = false;
                tb.BackColor = Color.Gray;
                flowLayoutPanel6.Controls.Remove(tb);
                flowLayoutPanel7.Controls.Add(tb);
            }
            else
            {
                tb.selected = true;
                tb.BackColor = Color.Green;
                flowLayoutPanel7.Controls.Remove(tb);
                flowLayoutPanel6.Controls.Add(tb);
            }
        }
        internal class tag_button:Label
        {
            public bool selected = false;
            public tag_button(string text)
            {
                Text = text;
                BackColor = Color.Gray;
                ForeColor = Color.White;
                AutoSize = true;
            }

        }
        HashSet<string> tag_set = new HashSet<string>();
        private void ImportForm_Load(object sender, EventArgs e)
        {
            useful.get_dir_or_create(Dir.FullName + "/files");
            foreach(var conf in Dir.GetDirectories("datas")[0].GetFiles())
            {
                using(StreamReader sr = new StreamReader(conf.FullName))
                {
                    string[] tags = sr.ReadLine().Split(' ');
                    foreach(var t in tags)
                    {
                        bool flag = tag_set.Add(t);
                        if (flag)
                        {
                            var tb = new tag_button(t);
                            tb.Click += tag_on_click;
                            flowLayoutPanel7.Controls.Add(tb);
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("請出入標籤");
            }
            else if(textBox2.Text.IndexOf(' ') != -1)
            {
                MessageBox.Show("標籤不能包含空格");
            }
            else if (textBox2.Text == "未知")
            {
                MessageBox.Show("標籤不可是 \"未知\"");
            }
            else
            {
                bool flag = tag_set.Add(textBox2.Text);
                if (flag)
                {
                    tag_button tb = new tag_button(textBox2.Text);
                    tb.BackColor = Color.Green;
                    tb.selected = true;
                    tb.Click += tag_on_click;
                    flowLayoutPanel6.Controls.Add(tb);
                }
                else
                {
                    foreach (tag_button tb in flowLayoutPanel7.Controls)
                    {
                        if (tb.Text == textBox2.Text)
                        {
                            tag_on_click(tb, null);
                            break;
                        }
                    }
                }
            }
            textBox2.Text = "";
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach(var file in openFileDialog1.FileNames)
                {
                    flowLayoutPanel8.Controls.Add(new FileBox(file));
                }
            }
        }

        public List<string> files = new List<string>();
        public List<string> tags = new List<string>();
        public string packname
        {
            get
            {
                return textBox1.Text;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("名稱不為空");
                return;
            }
            if (flowLayoutPanel6.Controls.Count == 0)
            {
                MessageBox.Show("至少要有一標籤");
                return;
            }
            if (flowLayoutPanel8.Controls.Count == 0)
            {
                MessageBox.Show("至少要有一檔案");
                return;
            }
            if (Dir.GetDirectories("datas")[0].GetFiles(textBox1.Text + ".txt").Length > 0) 
            {
                if (MessageBox.Show("要覆寫 " + textBox1.Text + " 嗎?", "警告", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    Close();
                    return;
                }
            }
            foreach (FileBox fb in flowLayoutPanel8.Controls)
            {
                files.Add(fb.SingleName);
                File.Copy(fb.FullName, Dir.FullName + "/files/" + fb.SingleName, true);
            }
            foreach (tag_button tb in flowLayoutPanel6.Controls) 
            {
                tags.Add(tb.Text);
            }
            using (StreamWriter sw = new StreamWriter(Dir.FullName + "/datas/" + textBox1.Text + ".txt"))
            {
                for(int i = 0; i < tags.Count - 1; i++)
                {
                    sw.Write(tags[i] + " ");
                }
                sw.WriteLine(tags.Last());
                foreach(var f in files)
                {
                    sw.WriteLine(f);
                }
            }
            MessageBox.Show("匯入完成");
            Close();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }
    }
}
