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

namespace MinecraftServers.Forms
{
    public partial class NewServer : Form
    {
        public NewServer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("請輸入地圖名稱");
                return;
            }
            string des = "./versions/" + comboBox1.Text + "/saves/" + textBox2.Text;
            string ori = "./versions/" + comboBox1.Text + "/ins";
            useful.get_dir_or_create(des);
            useful.CopyDirectory(ori, des, true);
            Close();
        }

        private void NewServer_Load(object sender, EventArgs e)
        {
            var vs = useful.get_dir_or_create("./versions");
            foreach (var v in vs.GetDirectories())
            {
                if (File.Exists(v.FullName + "/config.json"))
                {
                    comboBox1.Items.Add(v.Name);
                }
            }
        }
    }
}
