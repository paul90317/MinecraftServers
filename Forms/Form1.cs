using MinecraftServers.Controls;
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
using MinecraftServers.Forms;

namespace MinecraftServers
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void enter_server(object sender, EventArgs e)
        {
            MenuButton mb = (MenuButton)sender;
            serverForm sf = new serverForm();
            sf.serverDir = useful.get_dir_or_create("./versions/" + mb.version + "/saves/" + mb.entry);
            Program.now = sf;
            Close();
            return;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            NewVersion form = new NewVersion();
            form.ShowDialog();
            Form1_Load(sender, e);
            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DirectoryInfo dir = useful.get_dir_or_create("./versions");
            while (flowLayoutPanel1.Controls.Count > 0)
            {
                flowLayoutPanel1.Controls[0].Dispose();
            }
            foreach (var vdir in dir.GetDirectories())
            {
                var maps = useful.get_dir_or_create(vdir.FullName + "/saves");
                foreach(var mdir in maps.GetDirectories())
                {
                    MenuButton mb = new MenuButton(mdir.Name, vdir.Name);
                    mb.add_click_eh(enter_server);
                    flowLayoutPanel1.Controls.Add(mb);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.now = new menu();
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NewServer nsf = new NewServer();
            nsf.ShowDialog();
            Form1_Load(sender, e);
            return;
        }
    }
}
