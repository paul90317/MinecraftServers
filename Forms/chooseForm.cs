using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftServers.Forms
{
    public partial class chooseForm : Form
    {
        public chooseForm()
        {
            InitializeComponent();
        }

        string tmp = null;
        private void button1_Click(object sender, EventArgs e)
        {
            tmp = comboBox1.Text;
            Close();
        }
        public string ShowDialag_and_return(string msg, List<string> choises)
        {
            label1.Text = msg;
            while (comboBox1.Items.Count > 0)
            {
                comboBox1.Items.RemoveAt(0);
            }
            foreach(string c in choises)
            {
                comboBox1.Items.Add(c);
            }
            tmp = null;
            ShowDialog();
            if (tmp == "") return null;
            return tmp;
        }
    }
}
