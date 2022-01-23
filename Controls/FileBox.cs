using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MinecraftServers.Controls
{
    public partial class FileBox : UserControl
    {
        public FileBox()
        {
            InitializeComponent();
        }
        string filename;
        public FileBox(string fullname)
        {
            InitializeComponent();
            filename = fullname;
            FileInfo file = new FileInfo(fullname);
            label1.Text = file.Name;
        }
        public string FullName
        {
            get
            {
                return filename;
            }
        }
        public string SingleName
        {
            get
            {
                return label1.Text;
            }
        }
        private void FileBox_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
