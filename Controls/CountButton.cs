using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftServers.Controls
{
    public partial class CountButton : UserControl
    {
        public CountButton()
        {
            InitializeComponent();
        }
        bool selected = false;
        int count = 0;
        ControlCollection cc_unsel, cc_sel;
        List<string> files;
        Dictionary<string, int> file_mods_cnt;
        public CountButton(string entry, string[] tags, ControlCollection ControlsSel, ControlCollection ControlsUnSel, List<string> _files, Dictionary<string, int> _file_mods_cnt)
        {
            InitializeComponent();
            label1.Text = entry;
            foreach (var t in tags)
            {
                Label lb = new Label();
                lb.AutoSize = true;
                lb.BackColor = Color.Green;
                lb.ForeColor = Color.White;
                lb.Text = t;
                lb.Click += label1_Click;
                flowLayoutPanel1.Controls.Add(lb);
            }
            Hide();
            cc_sel = ControlsSel;
            cc_unsel = ControlsUnSel;
            cc_unsel.Add(this);
            file_mods_cnt = _file_mods_cnt;
            files = _files;
        }
        
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void sel()
        {
            selected = true;
            Show();
            cc_unsel.Remove(this);
            cc_sel.Add(this);
            foreach(string filename in files)
            {
                if (!file_mods_cnt.ContainsKey(filename))
                {
                    file_mods_cnt[filename] = 0;
                }
                file_mods_cnt[filename]++;
            }
        }
        public void unsel()
        {
            selected = false;
            if (count == 0)
            {
                Hide();
            }
            cc_sel.Remove(this);
            cc_unsel.Add(this);
            foreach (string filename in files)
            {
                file_mods_cnt[filename]--;
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {
            if (selected == true)
            {
                unsel();
            }
            else
            {
                sel();
            }
        }
        public void showing()
        {
            count++;
            Show();
        }
        public void hiding()
        {
            count--;
            if (count == 0 && selected == false)
            {
                Hide();
            }
        }

        private void CountButton_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public string packname
        {
            get
            {
                return label1.Text;
            }
        }
    }
}
