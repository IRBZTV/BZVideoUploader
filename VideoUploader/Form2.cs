using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VideoUploader
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MyDBTableAdapters.ARCHIVETableAdapter Ta = new MyDBTableAdapters.ARCHIVETableAdapter();
            Ta.InserProg(textBox1.Text.Trim(), (short)(comboBox1.SelectedIndex + 1));
            MessageBox.Show("برنامه اضافه شد لطفا لیست را بازیابی کنید");
            this.Close();
        }
    }
}
