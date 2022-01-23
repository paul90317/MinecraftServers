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
    public partial class properties_show_choose : Form
    {
        public properties_show_choose()
        {
            InitializeComponent();
        }
        public DirectoryInfo serverDir;
        private void properties_show_choose_Load(object sender, EventArgs e)
        {
            if (serverDir.GetFiles("server.properties").Length == 0)
            {
                MessageBox.Show("找不到 server.properties");
                Close();
                return;
            }
            FileInfo file = serverDir.GetFiles("server.properties")[0];
            using (StreamReader sr = new StreamReader(file.FullName)) 
            {
                
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line[0] == '#') continue;
                    string entry = line.Split('=')[0];
                    FlowLayoutPanel flp = new FlowLayoutPanel();
                    flp.BackColor = Color.Transparent;
                    flp.AutoSize = true;
                    flp.Dock = DockStyle.Top;
                    if (!Program.properties_show.ContainsKey(entry))
                    {
                        var vl = new ViewList(entry);
                        vl.unselect();
                        flp.Controls.Add(vl);
                    }
                    else
                    {
                        var vl = new ViewList(entry, Program.properties_show[entry]);
                        vl.select();
                        flp.Controls.Add(vl);
                    }
                    panel3.Controls.Add(flp);
                }
               
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Program.properties_show = new Dictionary<string, string[]>();
            foreach(FlowLayoutPanel c in panel3.Controls)
            {
                ViewList vl = (ViewList)c.Controls[0];
                string[] arr = vl.values;
                if (arr == null)
                {
                    continue;
                }
                else
                {
                    Program.properties_show[vl.entry_name] = arr;
                }
            }
            MessageBox.Show("儲存完成");
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
