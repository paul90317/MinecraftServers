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
    public partial class InputBox : UserControl
    {
        public InputBox(string entry_name, string value)
        {
            InitializeComponent();
            label1.Text = entry_name;
            textBox1.Text = value;
        }
        public string value
        {
            get
            {
                return textBox1.Text;
            }
        }
        public string key
        {
            get
            {
                return label1.Text;
            }
        }
    }
}