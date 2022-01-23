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
    public partial class SwitchButton : UserControl
    {
        public int id = -1;
        public string[] options = new string[0];
        public string entry_name = "hello";
        public string key
        {
            get
            {
                return entry_name;
            }
        }
        public string value
        {
            get
            {
                if (id == -1) return "";
                return options[id];
            }
        }
        public SwitchButton(string _entry_name, string[] _options, string selection)
        {
            InitializeComponent();
            entry_name = _entry_name;
            options = _options;
            id = Array.IndexOf(options, selection);
            button1.Text = entry_name + ": " + selection;
        }
        public SwitchButton()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            id = (id + 1) % options.Length;
            button1.Text = entry_name + ": " + options[id];
        }
    }
}
