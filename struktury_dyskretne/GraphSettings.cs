using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace struktury_dyskretne
{
    public partial class GraphSettings : Form
    {
        public GraphSettings()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int cityRadius = Convert.ToInt32(textBox1.Text);
            int transmitersNumber = Convert.ToInt32(textBox2.Text);
            int transmiterRange = Convert.ToInt32(textBox3.Text);
            GenerateGraph picture = new GenerateGraph(cityRadius, transmitersNumber, transmiterRange);
            picture.Width = 2 * cityRadius + 100;
            picture.Height = 2 * cityRadius + 100;
            picture.Show();
        }
    }
}
