using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NewsStillsRnd
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult Rs = MessageBox.Show("آیا از برنامه خارج می شوید", "خروج از برنامه",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Rs == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }   
        }
    }
}
