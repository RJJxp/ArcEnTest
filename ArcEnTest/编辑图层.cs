using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArcEnTest
{
    public partial class 编辑图层 : Form
    {
        public   编辑图层()
        {
            InitializeComponent();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button1.Enabled = true;
            button4.Enabled = false;
            button3.Enabled = true;

        }

        private void 编辑图层_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button2.Enabled = false;
            button1.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1.flagCreateFeature = false;
            Form1.flagDeleteFeature = false;
            button3.Enabled = false;
            button2.Enabled = false;
            button1.Enabled = false;
            button4.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.flagCreateFeature = true;
            Form1.flagDeleteFeature = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.flagDeleteFeature = true;
            Form1.flagCreateFeature = false;
        }
    }
}
