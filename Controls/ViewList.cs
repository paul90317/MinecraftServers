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
    public partial class ViewList : UserControl
    {
        public ViewList(string entry, string[] values)
        {
            InitializeComponent();
            label1.Text = entry;
            foreach(var v in values)
            {
                flowLayoutPanel2.Controls.Add(new_b(v));
            }
        }
        public ViewList(string entry)
        {
            InitializeComponent();
            label1.Text = entry;
        }
        void on_click(object sender, EventArgs e)
        {
            flowLayoutPanel2.Controls.Remove((Control)sender);
        }
        Label new_b(string text)
        {
            Label b = new Label();
            b.Text = text;
            b.BackColor = Color.DarkGreen;
            b.ForeColor = Color.White;
            b.AutoSize = true;
            b.Click += on_click;
            return b;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("不可以空白");
                return;
            }
            if (textBox1.Text.IndexOf(' ') != -1) 
            {
                MessageBox.Show("不可以有空格");
                return;
            }
            flowLayoutPanel2.Controls.Add(new_b(textBox1.Text));
            textBox1.Text = "";
        }
        public string[] values
        {
            get
            {
                if (selected == false) return null;
                string[] arr = new string[flowLayoutPanel2.Controls.Count];
                for(int i = 0; i < flowLayoutPanel2.Controls.Count; i++)
                {
                    arr[i] = flowLayoutPanel2.Controls[i].Text;
                }
                return arr;
            }
        }
        public string entry_name
        {
            get
            {
                return label1.Text;
            }
        }
        bool selected = true;
        public void select()
        {
            selected = true;
            button1.Show();
            textBox1.Show();
            flowLayoutPanel2.Show();
        }
        public void unselect()
        {
            selected = false;
            button1.Hide();
            textBox1.Hide();
            flowLayoutPanel2.Hide();
        }
        private void ViewList_Load(object sender, EventArgs e)
        {
            //unselect();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (selected)
            {
                unselect();
            }
            else select();
            textBox1.Focus();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
