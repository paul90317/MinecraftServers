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
    public partial class MenuButton : UserControl
    {
        public string entry
        {
            get
            {
                return label1.Text;
            }
        }
        public string version
        {
            get
            {
                return _vs;
            }
        }
        string _vs;
        public Dictionary<string, string> datas = new Dictionary<string, string>();
        public MenuButton(string _entry, string version)
        {
            InitializeComponent();
            label1.Text = _entry;
            _vs = version;
            label2.Text = "版本: " + version;
        }

        private void MenuButton_Load(object sender, EventArgs e)
        {

        }
        List<EventHandler> ehs = new List<EventHandler>();
        public void add_click_eh(EventHandler eh)
        {
            ehs.Add(eh);
            Click += eh;
        }
        public Color foreColor
        {
            set
            {
                label1.ForeColor = value;
                label2.ForeColor = value;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            foreach(var eh in ehs)
            {
                eh(this, e);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            foreach (var eh in ehs)
            {
                eh(this, e);
            }
        }
    }
}
