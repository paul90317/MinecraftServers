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

namespace MinecraftServers
{
    public partial class NewVersion : Form
    {
        public NewVersion()
        {
            InitializeComponent();
        }

        private void createServer_Load(object sender, EventArgs e)
        {
            
        }

        private void flowLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("請輸入版本名稱");
                return;
            }
            try
            {
                versionConfig vc = new versionConfig();
                vc.javapath = textBox2.Text;
                FileInfo jarf = new FileInfo(textBox3.Text);
                vc.jarfile = jarf.FullName;
                var dir = useful.get_dir_or_create("./versions/" + textBox1.Text);
                string ins_dir = dir.FullName + "/ins";
                if (Directory.Exists(ins_dir))
                {
                    Directory.Delete(ins_dir, true);
                }
                useful.get_dir_or_create(dir.FullName + "/saves");
                string configf = dir.FullName + "/config.json";
                if (File.Exists(configf))
                {
                    MessageBox.Show("該版本已經存在");
                    return;
                }
                using (StreamWriter sw = new StreamWriter(configf))
                {
                    string data = JsonSerializer.Serialize(vc);
                    sw.Write(data);
                }
                useful.CopyDirectory(jarf.Directory.FullName, ins_dir, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JAVA|java.exe";
            
            try
            {
                FileInfo file = new FileInfo(textBox2.Text);
                if (file.Exists)
                {
                    openFileDialog1.InitialDirectory = file.Directory.FullName;
                }
            }
            catch (Exception)
            {
                openFileDialog1.InitialDirectory = "";
            }
            finally
            {
                var result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textBox2.Text = openFileDialog1.FileName;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JAR|*.jar|BAT|*.bat";
            try
            {
                FileInfo file = new FileInfo(textBox3.Text);
                if (file.Exists)
                {
                    openFileDialog1.InitialDirectory = file.Directory.FullName;
                }
            }
            catch (Exception)
            {
                openFileDialog1.InitialDirectory = "";
            }
            finally
            {
                var result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textBox3.Text = openFileDialog1.FileName;
                }
            }
        }
    }
}
